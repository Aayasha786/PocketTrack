public class TransactionService
{
    public List<TransactionModel> Transactions { get; private set; } = new();

    public void AddTransaction(TransactionModel transaction)
    {
        Transactions.Add(transaction);
    }

    public void RemoveTransaction(TransactionModel transaction)
    {
        Transactions.Remove(transaction);
    }

    public decimal GetTotalInflows() =>
        Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);

    public decimal GetTotalOutflows() =>
        Transactions.Where(t => t.Type == "Expense").Sum(t => Math.Abs(t.Amount));

    public decimal GetTotalDebts() =>
        Transactions.Where(t => t.Type == "Debt").Sum(t => Math.Abs(t.Amount));

    public decimal GetClearedDebt() =>
        Transactions.Where(t => t.Type == "Debt Payment").Sum(t => Math.Abs(t.Amount));

    public decimal GetRemainingDebt() =>
        GetTotalDebts() - GetClearedDebt();

    public decimal GetTotalBalance()
    {
        return GetTotalInflows() - GetTotalOutflows() - GetTotalDebts();
    }

    public TransactionModel GetHighestTransaction(string type) =>
        Transactions.Where(t => t.Type == type).OrderByDescending(t => t.Amount).FirstOrDefault();

    public TransactionModel GetLowestTransaction(string type) =>
        Transactions.Where(t => t.Type == type).OrderBy(t => t.Amount).FirstOrDefault();
}


public class TransactionModel
{
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } // Income, Expense, Debt, Debt Payment
    public List<string> Tags { get; set; } // Custom tags for transactions
    public string Note { get; set; } // Optional note for the transaction
}
