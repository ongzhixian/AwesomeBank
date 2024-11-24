using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass]
public class InputTransactionsTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();
    private readonly BankingService bankingService = new(dataAccess);

    [TestMethod]
    public void WhenStoreAccountTransaction_DataAccessSaveAccountTransaction()
    {
        dataAccess.LoadAccountTransactions(Arg.Any<string>())
            .Returns(callInfo => Enumerable.Empty<AccountTransaction>().ToList());

        AccountTransaction accountTransaction = new AccountTransaction
        {
            Id = "TEST-01",
            AccountId = "ACTEST",
            Date = new DateOnly(2020, 6, 1),
            TransactionType = 'D',
            Amount = 500
        };

        bankingService.StoreAccountTransaction(accountTransaction);

        dataAccess.Received(1).SaveAccountTransaction(Arg.Any<AccountTransaction>());
    }


    [TestMethod]
    public void WhenHasAccountTransactions_DisplayAccountTransactions()
    {
        List<AccountTransaction> accountTransactions =
        [
            new()
            {
                AccountId = "ACTEST2",
                Date = new DateOnly(2020, 6, 1), Id = "TEST-TXN-01", TransactionType = 'D', Amount = 100
            }
        ];

        dataAccess.LoadAccountTransactions(Arg.Any<string>()).Returns(callInfo => accountTransactions);

        var originalConsoleOut = Console.Out;

        using var outputStringStore = new StringWriter();

        Console.SetOut(outputStringStore);

        bankingService.PrintAccountTransactions("ACTEST2");

        Console.SetOut(originalConsoleOut); // Restore

        Assert.AreEqual(
            "Account: ACTEST2\r\n| Date     | Txn Id       | Type |   Amount |\r\n| 20200601 | TEST-TXN-01  | D    |   100.00 |\r\n",
            outputStringStore.ToString());
    }

    [TestMethod]
    public void WhenHasAccountTransactions_DisplayAccountTransactionsHeadersOnly()
    {
        List<AccountTransaction> accountTransactions =
        [
        ];

        dataAccess.LoadAccountTransactions(Arg.Any<string>()).Returns(callInfo => accountTransactions);

        var originalConsoleOut = Console.Out;

        using var outputStringStore = new StringWriter();

        Console.SetOut(outputStringStore);

        bankingService.PrintAccountTransactions("ACTEST2");

        Console.SetOut(originalConsoleOut); // Restore

        Assert.AreEqual("Account: ACTEST2\r\n| Date     | Txn Id       | Type |   Amount |\r\n", outputStringStore.ToString());
    }
}