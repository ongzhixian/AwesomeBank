using ConsoleApp;
using NSubstitute;

namespace ConsoleAppTests;

[TestClass]
public class DefineInterestRulesTests
{
    private static readonly IBankingServicesDataAccess dataAccess = Substitute.For<IBankingServicesDataAccess>();
    
    private readonly BankingService bankingService = new BankingService(dataAccess);
    
    [TestMethod]
    public void WhenStoreInterestRule_DataAccessSaveInterestRates()
    {
        dataAccess.LoadInterestRates().Returns(callInfo => Enumerable.Empty<InterestRate>().ToList());
        
        InterestRate interestRate = new InterestRate
        {
            RuleId = "TEST01",
            EffectiveDate = new DateOnly(2020, 01, 01),
            Rate = 5.00M    
        };
        
        bankingService.StoreInterestRule(interestRate);
        
        dataAccess.Received(1).SaveInterestRates(Arg.Any<List<InterestRate>>());
    }

    [TestMethod]
    public void WhenInterestRulesDefined_DisplayInterestRules()
    {
        List<InterestRate> interestRates =
        [
            new() { EffectiveDate = new DateOnly(2023, 01, 21), Rate = 1.95M, RuleId = "RULE01" },
        ];

        dataAccess.LoadInterestRates().Returns(callInfo => interestRates);
        
        var originalConsoleOut = Console.Out;
        
        using var outputStringStore = new StringWriter();
        
        Console.SetOut(outputStringStore);
        
        bankingService.PrintInterestRules();
        
        Console.SetOut(originalConsoleOut); // Restore
        
        Assert.AreEqual("Interest rules:\r\n| Date     | RuleId   | Rate (%) |\r\n| 20230121 | RULE01   |     1.95 |\r\n", outputStringStore.ToString());
    }
    
    [TestMethod]
    public void PrintInterestRulesWithNoInterestRulesDefined_DisplayInterestRulesHeadersOnly()
    {
        List<InterestRate> interestRates = [];

        dataAccess.LoadInterestRates().Returns(callInfo => interestRates);
        
        var originalConsoleOut = Console.Out;
        
        using var outputStringStore = new StringWriter();
        
        Console.SetOut(outputStringStore);
        
        bankingService.PrintInterestRules();
        
        Console.SetOut(originalConsoleOut); // Restore
        
        Assert.AreEqual("Interest rules:\r\n| Date     | RuleId   | Rate (%) |\r\n", outputStringStore.ToString());
    }
}