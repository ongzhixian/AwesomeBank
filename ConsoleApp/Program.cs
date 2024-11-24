using ConsoleApp;
using UI = ConsoleApp.UserInterface;

bool continueSession = true;
bool showWelcome = true;

BankingService bankingService = new BankingService(new DataFileAccess());

while (continueSession)
{
    UI.DisplayUserActionMenu(ref showWelcome);
    var userAction =  Console.ReadLine()?.Trim().ToUpperInvariant();
    
    switch (userAction)
    {
        case "T":
            InputTransactions();
            break;
        case "I":
            DefineInterestRules();
            break;
        case "P":
            PrintStatement();
            break;
        case "Q":
            UI.DisplayExitMessage();
            continueSession = false;
            break;
    }

}

return;


void InputTransactions()
{
    Console.Clear();
    Console.Write("""
                  Please enter transaction details in <Date> <Account> <Type> <Amount> format 
                  (or enter blank to go back to main menu):
                  >
                  """);
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) return;
    
    try
    {
        var accountTransaction = bankingService.ValidateTransactionInput(userInput);
        
        bankingService.StoreAccountTransaction(accountTransaction);
        
        bankingService.PrintAccountTransactions(accountTransaction.AccountId);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

void DefineInterestRules()
{
    Console.Clear();
    Console.Write("""
                  Please enter interest rules details in <Date> <RuleId> <Rate in %> format 
                  (or enter blank to go back to main menu):
                  >
                  """);
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) return;
    
    try
    {
        var interestRule = bankingService.ValidateInterestRuleInput(userInput);
        
        bankingService.StoreInterestRule(interestRule);

        bankingService.PrintInterestRules();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

void PrintStatement()
{
    Console.Clear();
    Console.Write("""
                  Please enter account and month to generate the statement <Account> <Year><Month>
                  (or enter blank to go back to main menu):
                  >
                  """);
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) return;
    
    try
    {
        var interestCalculationCriteria = bankingService.ValidatePrintStatementInput(userInput);
        
        bankingService.PrintAccountStatements(interestCalculationCriteria);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    
}
