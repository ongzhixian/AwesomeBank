using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass]
public class PrintAccountTransactionsTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();

    private readonly BankingService bankingService = new(dataAccess);

    [TestMethod]
    public void WhenPrintAccountTransactionsWithNoInterestRulesAndAccountTransactions_PrintAccountStatementsHeadersOnly()
    {
        dataAccess.LoadInterestRates().Returns(callInfo => Enumerable.Empty<InterestRate>().ToList());
        dataAccess.LoadAccountTransactions(Arg.Any<string>()).Returns(callInfo => Enumerable.Empty<AccountTransaction>().ToList());
        
        InterestCalculationCriteria interestCalculationCriteria = new InterestCalculationCriteria
        {
            AccountId = "AC001",
            YearMonthStartDate = new DateOnly(2023, 6, 1)
        };

        var originalConsoleOut = Console.Out;

        using var outputStringStore = new StringWriter();

        Console.SetOut(outputStringStore);

        bankingService.PrintAccountStatements(interestCalculationCriteria);

        Console.SetOut(originalConsoleOut);

        Assert.AreEqual("Account: AC001\r\n| Date     | Txn Id       | Type |   Amount |  Balance |\r\n", outputStringStore.ToString());
    }
    
    [TestMethod]
    public void WhenPrintAccountTransactions_DisplayExitMessage()
    {
        // 20230101 RULE01 1.95
        // 20230520 RULE02 1.90
        // 20230615 RULE03 2.20
        List<InterestRate> interestRates =
        [
            new () { EffectiveDate = new DateOnly(2023, 1, 1), RuleId = "RULE01", Rate = 1.95M },
            new () { EffectiveDate = new DateOnly(2023, 5, 20), RuleId = "RULE02", Rate = 1.90M },
            new () { EffectiveDate = new DateOnly(2023, 6, 15), RuleId = "RULE03", Rate = 2.20M },
        ];
        
        // 20230501 AC001 D 100.00
        // 20230601 AC001 D 150.00
        // 20230626 AC001 W 20.00
        // 20230626 AC001 W 100.00
        List<AccountTransaction> accountTransactions =
        [
            new() { AccountId = "AC001", Date = new DateOnly(2023, 5, 1), Id="20230501-01", TransactionType = 'D', Amount = 100.00M },
            new() { AccountId = "AC001", Date = new DateOnly(2023, 6, 1), Id="20230601-01", TransactionType = 'D', Amount = 150.00M },
            new() { AccountId = "AC001", Date = new DateOnly(2023, 6, 26), Id="20230626-01", TransactionType = 'W', Amount = 20.00M },
            new() { AccountId = "AC001", Date = new DateOnly(2023, 6, 26), Id="20230626-02", TransactionType = 'W', Amount = 100.00M }
        ];
        
        dataAccess.LoadInterestRates().Returns(callInfo => interestRates);
        dataAccess.LoadAccountTransactions(Arg.Any<string>()).Returns(callInfo => accountTransactions);
        
        InterestCalculationCriteria interestCalculationCriteria = new InterestCalculationCriteria
        {
            AccountId = "AC001",
            YearMonthStartDate = new DateOnly(2023, 6, 1)
        };

        var originalConsoleOut = Console.Out;

        using var outputStringStore = new StringWriter();

        Console.SetOut(outputStringStore);

        bankingService.PrintAccountStatements(interestCalculationCriteria);

        Console.SetOut(originalConsoleOut);

        Assert.AreEqual( "Account: AC001\r\n| Date     | Txn Id       | Type |   Amount |  Balance |\r\n| 20230601 | 20230601-01  | D    |   150.00 |   250.00 |\r\n| 20230626 | 20230626-01  | W    |    20.00 |   230.00 |\r\n| 20230626 | 20230626-02  | W    |   100.00 |   130.00 |\r\n| 20230630 |              | I    |     0.39 |   130.39 |\r\n", outputStringStore.ToString());
    }
}