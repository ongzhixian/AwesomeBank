namespace ConsoleApp;

internal static class UserInterface
{
    public static void DisplayExitMessage()
    {
        Console.WriteLine("""
                          Thank you for banking with AwesomeGIC Bank.
                          Have a nice day!
                          """);
    }

    public static void DisplayUserActionMenu(ref bool showWelcome)
    {
        if (showWelcome)
        {
            Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
            showWelcome = false;
        }
        else
        {
            Console.WriteLine("Is there anything else you'd like to do?");
        }

        Console.Write("""
                          [T] Input transactions
                          [I] Define interest rules
                          [P] Print statement
                          [Q] Quit
                          >
                          """);
    }

    public static void DisplayInterestRules(IEnumerable<InterestRate> interestRates)
    {
        Console.WriteLine("Interest rules:");
        Console.WriteLine("| {0, -8} | {1, -8} | {2, 8} |", "Date", "RuleId", "Rate (%)");
        foreach (var interestRate in interestRates)
            Console.WriteLine("| {0, -8:yyyyMMdd} | {1, -8} | {2, 8:F2} |", interestRate.EffectiveDate,
                interestRate.RuleId, interestRate.Rate);
    }

    public static void DisplayAccountTransactions(string accountId, IEnumerable<AccountTransaction> accountTransactions)
    {
        Console.WriteLine("Account: {0}", accountId);
        Console.WriteLine("| {0, -8} | {1, -12} | {2, -4} | {3, 8} |", "Date", "Txn Id", "Type", "Amount");
        foreach (var accountTransaction in accountTransactions)
            Console.WriteLine("| {0, -8:yyyyMMdd} | {1, -12} | {2, -4} | {3, 8:F2} |", accountTransaction.Date,
                accountTransaction.Id, accountTransaction.TransactionType, accountTransaction.Amount);
    }
}
