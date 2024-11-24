using ConsoleApp;

namespace ConsoleAppTests;

[TestClass]
public class UserInterfaceTests
{
    [TestMethod]
    public void WhenDisplayExitMessage_DisplayExitMessage()
    {
        var originalConsoleOut = Console.Out;
        
        using var outputStringStore = new StringWriter();
        
        Console.SetOut(outputStringStore);
        
        UserInterface.DisplayExitMessage();
        
        Console.SetOut(originalConsoleOut); // Restore
        
        Assert.AreEqual("Thank you for banking with AwesomeGIC Bank.\r\nHave a nice day!\r\n", outputStringStore.ToString());
    }
    
    [TestMethod]
    public void WhenDisplayUserActionMenuShowWelcome_DisplayActionMenuWithWelcome()
    {
        var originalConsoleOut = Console.Out;
        
        using var outputStringStore = new StringWriter();
        
        Console.SetOut(outputStringStore);

        bool showWelcome = true;
        UserInterface.DisplayUserActionMenu(ref showWelcome);
        
        Console.SetOut(originalConsoleOut); // Restore
        
        Assert.AreEqual("Welcome to AwesomeGIC Bank! What would you like to do?\r\n[T] Input transactions\r\n[I] Define interest rules\r\n[P] Print statement\r\n[Q] Quit\r\n>", outputStringStore.ToString());
    }
    
    [TestMethod]
    public void WhenDisplayUserActionMenuShowWelcome_DisplayActionMenuWithAnythingElse()
    {
        var originalConsoleOut = Console.Out;
        
        using var outputStringStore = new StringWriter();
        
        Console.SetOut(outputStringStore);

        bool showWelcome = false;
        UserInterface.DisplayUserActionMenu(ref showWelcome);
        
        Console.SetOut(originalConsoleOut); // Restore
        
        Assert.AreEqual("Is there anything else you'd like to do?\r\n[T] Input transactions\r\n[I] Define interest rules\r\n[P] Print statement\r\n[Q] Quit\r\n>", outputStringStore.ToString());
    }
    
}