namespace ConsoleApp;

public class TooLittleUserInputError(byte expectedTokenCount, byte actualTokenCount)
    : ArgumentException($"Too little tokens provided. Expected {expectedTokenCount} but got {actualTokenCount}.");

public class TooManyUserInputError(byte expectedTokenCount, byte actualTokenCount)
    : ArgumentException($"Too many tokens provided. Expected {expectedTokenCount} but got {actualTokenCount}.");

public class InvalidDateStringError(string dateString)
    : ArgumentException($"{dateString} is not valid date.");

public class InvalidYearMonthStringError(string yearMonthString)
    : ArgumentException($"{yearMonthString} is not valid year-month.");

public class InvalidInterestRateError(string interestRateString)
    : ArgumentException($"{interestRateString} is not valid interest rate. Interest rate must be greater than 0 and less than 100.");

public class InvalidAmountStringError(string amountString)
    : ArgumentException($"{amountString} is not valid input amount. Amount must be greater than 0 and up to 2 decimal places.");

public class InvalidOperationError(AccountTransaction accountTransaction)
    : ArgumentException($"{accountTransaction.TransactionType} is not valid operation.");

public class InvalidTransactionTypeError(string transactionTypeString)
    : ArgumentException($"{transactionTypeString} is not valid transaction type.");

public class MissingInterestRatesForDateError(DateOnly targetDate)
    : ArgumentException($"Interest rates missing for {targetDate}.");

public class InvalidCalculateBalanceTransactionTypeError(char transactionTypeChar)
    : ArgumentException($"Unable to calculate balance for transaction type {transactionTypeChar}.");



