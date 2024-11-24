using System.Text;
using System.Text.Json;

namespace ConsoleApp;

public interface IBankingServicesDataAccess
{
    List<AccountTransaction> LoadAccountTransactions(string accountTransactionAccountId);
    
    void SaveAccountTransaction(AccountTransaction accountTransaction);
    
    List<InterestRate> LoadInterestRates();
    
    void SaveInterestRates(List<InterestRate> interestRates);
}

public class DataFileAccess : IBankingServicesDataAccess
{
    const string DATA_DIRECTORY_PATH = "./DATA";
    
    static readonly string INTEREST_RATE_DATA_FILE_PATH = Path.Combine(DATA_DIRECTORY_PATH, "interest-rates.json");
    static readonly string ACCOUNT_TRANSACTIONS_DATA_FILE_PATH = Path.Combine(DATA_DIRECTORY_PATH, "account-transactions.json");
    
    static readonly List<AccountTransaction> EMPTY_ACCOUNT_TRANSACTION_LIST = Enumerable.Empty<AccountTransaction>().ToList();
    
    
    public List<AccountTransaction> LoadAccountTransactions(string accountTransactionAccountId)
    {
        return LoadAccountTransactionsFromFile(accountTransactionAccountId);
    }

    void IBankingServicesDataAccess.SaveAccountTransaction(AccountTransaction accountTransaction)
    {
        SaveAccountTransactions(accountTransaction);
    }

    public List<InterestRate> LoadInterestRates()
    {
        return LoadInterestRatesFromFile();
    }

    public void SaveInterestRates(List<InterestRate> interestRates)
    {
        SaveInterestRatesToFile(interestRates);
    }
    
    private static List<AccountTransaction> LoadAccountTransactionsFromFile(string accountTransactionAccountId)
    {
        EnsureDataDirectoryExists();
        
        if (!File.Exists(ACCOUNT_TRANSACTIONS_DATA_FILE_PATH)) return EMPTY_ACCOUNT_TRANSACTION_LIST;
        
        using var sr = new StreamReader(ACCOUNT_TRANSACTIONS_DATA_FILE_PATH, Encoding.UTF8);
        
        var result =  JsonSerializer.Deserialize<List<AccountTransaction>>(sr.ReadToEnd()) ?? EMPTY_ACCOUNT_TRANSACTION_LIST;
        
        return result.Where(r => r.AccountId == accountTransactionAccountId).ToList();
    }
    
    private static void SaveAccountTransactions(AccountTransaction accountTransaction)
    {
        EnsureDataDirectoryExists();

        List<AccountTransaction> allAccountTransactions = Enumerable.Empty<AccountTransaction>().ToList();
        
        if (File.Exists(ACCOUNT_TRANSACTIONS_DATA_FILE_PATH))
        {
            using var sr = new StreamReader(ACCOUNT_TRANSACTIONS_DATA_FILE_PATH, Encoding.UTF8);
            allAccountTransactions =  JsonSerializer.Deserialize<List<AccountTransaction>>(sr.ReadToEnd()) ?? EMPTY_ACCOUNT_TRANSACTION_LIST;
            sr.Close();    
        }
        
        allAccountTransactions.Add(accountTransaction);
        
        using var sw = new StreamWriter(ACCOUNT_TRANSACTIONS_DATA_FILE_PATH, false, Encoding.UTF8);
        sw.AutoFlush = true;
        sw.Write(JsonSerializer.Serialize(allAccountTransactions));
    }
    
    private static List<InterestRate> LoadInterestRatesFromFile()
    {
        EnsureDataDirectoryExists();
        
        if (!File.Exists(INTEREST_RATE_DATA_FILE_PATH)) return Enumerable.Empty<InterestRate>().ToList();
        
        using var sr = new StreamReader(INTEREST_RATE_DATA_FILE_PATH, Encoding.UTF8);
        
        var result =  JsonSerializer.Deserialize<List<InterestRate>>(sr.ReadToEnd()) ?? Enumerable.Empty<InterestRate>().ToList();
        
        return result;
    }

    private static void SaveInterestRatesToFile(List<InterestRate> interestRates)
    {
        EnsureDataDirectoryExists();
        
        using var sw = new StreamWriter(INTEREST_RATE_DATA_FILE_PATH, false, Encoding.UTF8);
        sw.AutoFlush = true;
        sw.Write(JsonSerializer.Serialize(interestRates));
    }
    
    private static void EnsureDataDirectoryExists()
    {
        if (!Directory.Exists(DATA_DIRECTORY_PATH))
            Directory.CreateDirectory(DATA_DIRECTORY_PATH);
    }

}


// public class MockBankingServicesDataAccess : IBankingServicesDataAccess
// {
//     private static byte[] interestRatesBytes = [];
//     private static byte[] accountTransactionBytes = [];
//     
//     public List<AccountTransaction> LoadAccountTransactions(string accountTransactionAccountId)
//     {
//         if (accountTransactionBytes.Length == 0) return Enumerable.Empty<AccountTransaction>().ToList();
//         
//         using var ms = new MemoryStream(accountTransactionBytes);
//         using var sr = new StreamReader(ms);
//         var result =  JsonSerializer.Deserialize<List<AccountTransaction>>(sr.ReadToEnd()) ?? Enumerable.Empty<AccountTransaction>().ToList();;
//         return result.Where(r => r.AccountId == accountTransactionAccountId).ToList();
//     }
//
//     public void SaveAccountTransaction(AccountTransaction accountTransaction)
//     {
//         using var ms = new MemoryStream();
//         using var sw = new StreamWriter(ms);
//         sw.AutoFlush = true;
//         sw.Write(JsonSerializer.Serialize(accountTransaction));
//         
//         accountTransactionBytes = ms.ToArray();
//     }
//
//     public List<InterestRate> LoadInterestRates()
//     {
//         if (interestRatesBytes.Length == 0) return Enumerable.Empty<InterestRate>().ToList();
//
//         using var ms = new MemoryStream(interestRatesBytes);
//         using var sr = new StreamReader(ms);
//         var result =  JsonSerializer.Deserialize<List<InterestRate>>(sr.ReadToEnd()) ?? Enumerable.Empty<InterestRate>().ToList();
//         return result;
//     }
//
//     public void SaveInterestRates(List<InterestRate> interestRates)
//     {
//         using var ms = new MemoryStream();
//         using var sw = new StreamWriter(ms);
//         sw.AutoFlush = true;
//         sw.Write(JsonSerializer.Serialize(interestRates));
//         
//         interestRatesBytes = ms.ToArray();
//         
//     }
// }