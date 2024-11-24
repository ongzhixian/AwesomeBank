using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass()]
public class ValidateTransactionInputTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();
    private readonly BankingService service = new(dataAccess);

    [TestMethod()]
    [DataRow("20230626 AC001 W 100.00")]
    public void WhenValidInput_ReturnTransaction(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }
    
    [TestMethod]
    [ExpectedException(typeof(TooLittleUserInputError))]
    [DataRow("20230626 AC001")]
    public void WhenUserProvideTooLittleInputTokens_ReturnTooLittleUserInputError(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }
    
    [TestMethod()]
    [ExpectedException(typeof(TooManyUserInputError))]
    [DataRow("20230626 AC001 W 100.00 SOMENOTE")]
    public void WhenUserProvideTooManyInputTokens_ReturnTooManyUserInputError(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }

    
    [TestMethod]
    [ExpectedException(typeof(InvalidDateStringError))]
    [DataRow("20231232 AC001 W 100.00")]
    public void WhenUserInputInvalidDate_ReturnInvalidDateString(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidAmountStringError))]
    [DataRow("20230626 AC001 W 100.001")]
    [DataRow("20230626 AC001 W -100.001")]
    public void WhenUserInputInvalidAmount_ReturnInvalidAmountStringError(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTransactionTypeError))]
    [DataRow($"20230626 TS001 X 100")]
    public void WhenUserInputInvalidTransactionType_ReturnInvalidTransactionTypeError(string userInput)
    {
        var result = service.ValidateTransactionInput(userInput);
        Assert.IsInstanceOfType<AccountTransaction>(result);
    }
}

