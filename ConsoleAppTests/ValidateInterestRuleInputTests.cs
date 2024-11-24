using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass()]
public class ValidateInterestRuleInputTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();
    private readonly BankingService service = new(dataAccess);

    [TestMethod()]
    [DataRow("20230615   RULE03   2.20")]
    [DataRow("20230615 RULE03 2.20")]
    public void WhenValidInput_ReturnInterestRate(string userInput)
    {
        var result = service.ValidateInterestRuleInput(userInput);
        Assert.IsInstanceOfType<InterestRate>(result);
    }
    
    [TestMethod]
    [ExpectedException(typeof(TooLittleUserInputError))]
    [DataRow("20230615 RULE03")]
    [DataRow("20230615")]
    public void WhenUserProvideTooLittleInputTokens_ReturnTooLittleUserInputError(string userInput)
    {
        var result = service.ValidateInterestRuleInput(userInput);
        Assert.IsInstanceOfType<InterestRate>(result);
    }
    
    [TestMethod()]
    [ExpectedException(typeof(TooManyUserInputError))]
    [DataRow("20230615 RULE03   2.20 SOMENOTE")]
    [DataRow("20230615  RULE03  2.20 SOME DESCRIPTION")]
    public void WhenUserProvideTooManyInputTokens_ReturnTooManyUserInputError(string userInput)
    {
        var result = service.ValidateInterestRuleInput(userInput);
        Assert.IsInstanceOfType<InterestRate>(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidInterestRateError))]
    [DataRow("20230615 RULE03 100.00")]
    [DataRow("20230615 RULE03 100.01")]
    [DataRow("20230615 RULE03 0")]
    public void WhenUserInputInvalidInterestRate_ReturnInvalidInterestRateError(string userInput)
    {
        var result = service.ValidateInterestRuleInput(userInput);
        Assert.IsInstanceOfType<InterestRate>(result);
    }

}