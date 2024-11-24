namespace ConsoleApp;

public class BankingService
{
    private readonly IBankingServicesDataAccess dataAccess;
    public BankingService(IBankingServicesDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

public InterestRate ValidateInterestRuleInput(string userInput)
    {
        const int expectedTokenCount = 3;

        var tokens = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var actualTokenCount = tokens.Length;

        switch (actualTokenCount)
        {
            case < expectedTokenCount:
                throw new TooLittleUserInputError((byte)actualTokenCount, expectedTokenCount);
            case > expectedTokenCount:
                throw new TooManyUserInputError((byte)actualTokenCount, expectedTokenCount);
        }

        DateOnly effectiveDate = ValidateDateString(tokens[0]);
        decimal interestRate = ValidateInterestRateString(tokens[2]);

        return new InterestRate
        {
            EffectiveDate = effectiveDate,
            RuleId = tokens[1],
            Rate = interestRate
        };
    }

    public void StoreInterestRule(InterestRate interestRate)
    {
        var interestRates = dataAccess.LoadInterestRates();

        interestRates.RemoveAll(r => r.EffectiveDate == interestRate.EffectiveDate);

        interestRates.Add(interestRate);

        dataAccess.SaveInterestRates(interestRates);
    }

    public void PrintInterestRules()
    {
        var interestRates = dataAccess.LoadInterestRates().OrderBy(r => r.EffectiveDate);

        UserInterface.DisplayInterestRules(interestRates);
    }

    // ACCOUNT TRANSACTION

    public AccountTransaction ValidateTransactionInput(string userInput)
    {
        const int expectedTokenCount = 4;

        var tokens = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var actualTokenCount = tokens.Length;

        if (actualTokenCount < expectedTokenCount)
            throw new TooLittleUserInputError((byte)actualTokenCount, expectedTokenCount);

        if (actualTokenCount > expectedTokenCount)
            throw new TooManyUserInputError((byte)actualTokenCount, expectedTokenCount);

        var accountId = tokens[1];
        var effectiveDate = ValidateDateString(tokens[0]);
        var transactionType = ValidateTransactionTypeString(tokens[2]);
        var transactionAmount = ValidateAmountString(tokens[3]);

        return new AccountTransaction
        {
            AccountId = accountId,
            Date = effectiveDate,
            TransactionType = transactionType,
            Amount = transactionAmount
        };
    }

    public void StoreAccountTransaction(AccountTransaction accountTransaction)
    {
        List<AccountTransaction> accountTransactionList =
            dataAccess.LoadAccountTransactions(accountTransaction.AccountId);

        if (!IsValidAccountOperation(accountTransactionList, accountTransaction))
            throw new InvalidOperationError(accountTransaction);
        
        accountTransaction.Id = GetTransactionId(accountTransactionList, accountTransaction); 
    
        dataAccess.SaveAccountTransaction(accountTransaction);    
        
    }

    public void PrintAccountTransactions(string accountTransactionAccountId)
    {
        var accountTransactions = dataAccess.LoadAccountTransactions(accountTransactionAccountId)
            .OrderBy(r => r.Date);

        UserInterface.DisplayAccountTransactions(accountTransactionAccountId, accountTransactions);
    }
    
    public void PrintAccountStatements(InterestCalculationCriteria interestCalculationCriteria)
    {
        var accountStatements = GetAccountStatements(interestCalculationCriteria);
        
        Console.WriteLine("Account: {0}", interestCalculationCriteria.AccountId);
        Console.WriteLine("| {0, -8} | {1, -12} | {2, -4} | {3, 8} | {4, 8} |", "Date", "Txn Id", "Type", "Amount", "Balance");
        foreach (var accountTransaction in accountStatements)
            Console.WriteLine("| {0, -8:yyyyMMdd} | {1, -12} | {2, -4} | {3, 8:F2} | {4, 8:F2} |", accountTransaction.Date,
                accountTransaction.Id, accountTransaction.TransactionType, accountTransaction.Amount, accountTransaction.Balance);
    }

    public InterestCalculationCriteria ValidatePrintStatementInput(string userInput)
    {
        const int expectedTokenCount = 2;

        var tokens = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var actualTokenCount = tokens.Length;

        if (actualTokenCount < expectedTokenCount)
            throw new TooLittleUserInputError((byte)actualTokenCount, expectedTokenCount);

        if (actualTokenCount > expectedTokenCount)
            throw new TooManyUserInputError((byte)actualTokenCount, expectedTokenCount);

        var accountId = tokens[0];
        var yearMonthStartDate = ValidateYearMonthString(tokens[1]);

        return new InterestCalculationCriteria
        {
            AccountId = accountId,
            YearMonthStartDate = yearMonthStartDate
        };
    }

    private List<AccountStatement> GetAccountStatements(InterestCalculationCriteria interestCalculationCriteria)
    {
        var accountTransactionList = dataAccess.LoadAccountTransactions(interestCalculationCriteria.AccountId);
        var interestRates = dataAccess.LoadInterestRates();
        
        if (accountTransactionList.Count == 0) return Enumerable.Empty<AccountStatement>().ToList();
        
        var lastBalance = GetAccountBalancePriorToYearMonth(accountTransactionList, interestCalculationCriteria.YearMonthStartDate);
        
        var accountStatements = GenerateAccountStatements(accountTransactionList, interestCalculationCriteria.YearMonthStartDate, lastBalance);

        // To calculate interest, we need:
        // 1. the last balance for prior to target period
        // 2. the list of transactions in target period
        // The steps to calculate interest is as follows:
        // For each day in target period, calculate EndOfDayAccountStatement = EOD Balance w/InterestRate 
        //   1. Determine the interest rate applicable for that day
        //   2. Determine the end-of-day balance after processing all transactions on that day
        
        var daysInTargetMonth = DateTime.DaysInMonth(interestCalculationCriteria.YearMonthStartDate.Year,
            interestCalculationCriteria.YearMonthStartDate.Month);

        List<EndOfDayAccountStatement> endOfDayAccountStatements = [];

        for (int dayNumber = 0; dayNumber < daysInTargetMonth; dayNumber++)
        {
            var targetDate = new DateOnly(
                interestCalculationCriteria.YearMonthStartDate.Year,
                interestCalculationCriteria.YearMonthStartDate.Month, dayNumber + 1);
            
            var interestRate = interestRates.Where(r => targetDate >= r.EffectiveDate).OrderByDescending(r => r.EffectiveDate).FirstOrDefault();
            
            if (interestRate == null) throw new MissingInterestRatesForDateError(targetDate);

            var transactionsOnTargetDate = accountTransactionList.Where(r => r.Date == targetDate).OrderBy(r => r.Id).ToList();

            if (transactionsOnTargetDate.Count == 0)
            {
                endOfDayAccountStatements.Add(new EndOfDayAccountStatement
                {
                    Date = targetDate,
                    Balance = lastBalance,
                    InterestRate = interestRate
                });
                
                continue;
            }
                 
            var totalDeposits = transactionsOnTargetDate.Where(r => r.TransactionType == 'D').Sum(r => r.Amount);
            var totalWithdrawals = transactionsOnTargetDate.Where(r => r.TransactionType == 'W').Sum(r => r.Amount);
            
            lastBalance = lastBalance + totalDeposits - totalWithdrawals;
            
            endOfDayAccountStatements.Add(new EndOfDayAccountStatement
            {
                Date = targetDate,
                Balance = lastBalance,
                InterestRate = interestRate
            });
            
        }

        // Now that we have EOD Balance w/InterestRate:
        // 1. Group records by interest rate and balance
        // 2. Calculate the interest for each group
        // 3. Sum up all the interests calculated (rounded to 2 decimal places)
        
        var interestsEarnedPerBalanceInterestRateGroup = from endOfDayAccountStatement in endOfDayAccountStatements
        group endOfDayAccountStatement by new { endOfDayAccountStatement.InterestRate, endOfDayAccountStatement.Balance } into g
        select new
        {
            //               Balance       * InterestRatePercent             * Days
            InterestEarned = g.Key.Balance * (g.Key.InterestRate.Rate / 100) * g.Count()
        };
        
        var annualizedInterest = interestsEarnedPerBalanceInterestRateGroup.Sum(r => r.InterestEarned / 365);
        annualizedInterest = Math.Round(annualizedInterest, 2);

        // Add an account statement for interest as the last statement

        lastBalance = lastBalance + annualizedInterest;
        
        accountStatements.Add(new AccountStatement
        {
            AccountId = interestCalculationCriteria.AccountId,
            Date = interestCalculationCriteria.YearMonthStartDate.AddMonths(1).AddDays(-1),
            Id = string.Empty,
            TransactionType = 'I',
            Amount = annualizedInterest,
            Balance = lastBalance 
        });

        return accountStatements;
    }

    private bool IsValidAccountOperation(List<AccountTransaction> accountTransactionList, AccountTransaction accountTransaction)
    {
        switch (accountTransaction.TransactionType)
        {
            case 'D':
                return true; // You can always deposit money
            case 'W':
            {
                var deposits = accountTransactionList.Where(r => r.TransactionType == 'D').Sum(r => r.Amount);
                var withdrawals = accountTransactionList.Where(r => r.TransactionType == 'W').Sum(r => r.Amount);
                var balance = deposits - withdrawals;
            
                return balance - accountTransaction.Amount >= 0;
            }
            default:
                return false; // Unknown operation; return false to be safe
        }
    }

    private string GetTransactionId(List<AccountTransaction> accountTransactionList, AccountTransaction accountTransaction)
    {
        var accountTransactionCountOnDate = accountTransactionList.Count(r => r.Date == accountTransaction.Date);
        
        return $"{accountTransaction.Date:yyyyMMdd}-{accountTransactionCountOnDate+1:0#}";
    }

    
    private List<AccountStatement> GenerateAccountStatements(List<AccountTransaction> accountTransactionList, DateOnly yearMonthStartDate, decimal lastBalance)
    {
        var accountStatements = new List<AccountStatement>(); 
        var endDate = yearMonthStartDate.AddMonths(1);
        var transactionsInPeriod = accountTransactionList.Where(r => r.Date >= yearMonthStartDate && r.Date <= endDate).ToList();
        
        if (transactionsInPeriod.Count == 0) return Enumerable.Empty<AccountStatement>().ToList();

        foreach (var transaction in transactionsInPeriod)
        {
            lastBalance = CalculateBalance(lastBalance, transaction.TransactionType, transaction.Amount);
            
            accountStatements.Add(new AccountStatement
            {
                AccountId = transaction.AccountId,
                Date = transaction.Date,
                Id = transaction.Id,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Balance = lastBalance
            });
        }

        return accountStatements;
    }

    private decimal CalculateBalance(decimal lastBalance, char transactionTransactionType, decimal transactionAmount)
    {
        if (transactionTransactionType == 'W')
            return lastBalance - transactionAmount;
        
        if (transactionTransactionType == 'D')
            return lastBalance + transactionAmount;
        
        
        throw new InvalidCalculateBalanceTransactionTypeError(transactionTransactionType);
    }

    private decimal GetAccountBalancePriorToYearMonth(List<AccountTransaction> accountTransactionList, DateOnly yearMonthStartDate)
    {
        var priorTransactions = accountTransactionList.Where(t => t.Date < yearMonthStartDate).ToList();

        if (!priorTransactions.Any()) return 0m;
        
        var totalWithdrawals = priorTransactions.Where(r => r.TransactionType == 'W').Sum(r => r.Amount);
        var totalDeposits = priorTransactions.Where(r => r.TransactionType == 'D').Sum(r => r.Amount);
        
        return totalDeposits - totalWithdrawals;
    }

    private char ValidateTransactionTypeString(string token)
    {
        return token.ToUpperInvariant() switch
        {
            "W" => 'W',
            "D" => 'D',
            _ => throw new InvalidTransactionTypeError(token)
        };
    }

    private decimal ValidateAmountString(string token)
    {
        if (decimal.TryParse(token, out decimal amount) && amount > 0 && amount.Scale <= 2) return amount;

        throw new InvalidAmountStringError(token);
    }

    private decimal ValidateInterestRateString(string token)
    {
        if (decimal.TryParse(token, out var interestRate) && interestRate is > 0 and < 100)
            return interestRate;

        throw new InvalidInterestRateError(token);
    }

    private DateOnly ValidateDateString(string token)
    {
        if (DateOnly.TryParseExact(token, "yyyyMMdd", out var date))
            return date;

        throw new InvalidDateStringError(token);
    }

    private DateOnly ValidateYearMonthString(string token)
    {
        if (DateOnly.TryParseExact(token, "yyyyMM", out var date))
            return date;

        throw new InvalidYearMonthStringError(token);
    }
    
}
