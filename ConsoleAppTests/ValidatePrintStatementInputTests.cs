using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass()]
public class ValidatePrintStatementInputTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();
    private readonly BankingService service = new(dataAccess);

    [TestMethod()]
    [DataRow("AC001 202306")]
    public void WhenValidInput_ReturnInterestCalculationCriteria(string userInput)
    {
        var result = service.ValidatePrintStatementInput(userInput);
        Assert.IsInstanceOfType<InterestCalculationCriteria>(result);
    }
    
    [TestMethod]
    [ExpectedException(typeof(TooLittleUserInputError))]
    [DataRow("AC001")]
    public void WhenUserProvideTooLittleInputTokens_ReturnTooLittleUserInputError(string userInput)
    {
        var result = service.ValidatePrintStatementInput(userInput);
        Assert.IsInstanceOfType<InterestCalculationCriteria>(result);
    }
    
    [TestMethod()]
    [ExpectedException(typeof(TooManyUserInputError))]
    [DataRow("AC001 202306 SOMENOTE")]
    public void WhenUserProvideTooManyInputTokens_ReturnTooManyUserInputError(string userInput)
    {
        var result = service.ValidatePrintStatementInput(userInput);
        Assert.IsInstanceOfType<InterestCalculationCriteria>(result);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidYearMonthStringError))]
    [DataRow("AC001 202313")]
    public void WhenUserInputInvalidYearMonth_ReturnInvalidYearMonthStringError(string userInput)
    {
        var result = service.ValidatePrintStatementInput(userInput);
        Assert.IsInstanceOfType<InterestCalculationCriteria>(result);
    }

}