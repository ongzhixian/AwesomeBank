namespace ConsoleApp;

public record InterestRate
{
    public required string RuleId { get; init; }
    
    public required DateOnly EffectiveDate { get; init; }

    public required decimal Rate { get; init; }
}

public record AccountTransaction
{
    public string Id { get; set; } = null!;

    public required string AccountId { get; init; }

    public required DateOnly Date { get; init; }

    public required char TransactionType { get; init; }

    public required decimal Amount { get; init; }
}

public record EndOfDayAccountStatement
{
    public DateOnly Date { get; set; }
    
    public decimal Balance { get; init; }
    
    public required InterestRate InterestRate { get; init; }
}

public record AccountStatement : AccountTransaction
{
    public required decimal Balance { get; init; }
}
    

public record InterestCalculationCriteria
{
    public required string AccountId { get; init; }

    public required DateOnly YearMonthStartDate { get; init; }    
}

// public record Account
// {
//     public required string Id { get; set; }
//
//     public required decimal Balance { get; set; }
//
//     public uint TransactionCount { get; set; }
// }
// public record DailyAccountTransactionBalance //: AccountTransaction
// {
//     public required DateOnly DailyBalanceDate { get; set; }
//
//     public required decimal? Balance { get; set; } = null;
// }

// | Date     | Txn Id      | Type | Amount | Balance |
// | 20230601 | 20230601-01 | D    | 150.00 |  250.00 |
