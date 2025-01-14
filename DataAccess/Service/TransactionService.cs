using ProjectTrack.Models;

public class TransactionService
{
    public List<TransactionModel> Transactions { get; private set; } = new();

    // Summary Properties
    public decimal TotalInflows => Transactions
        .Where(t => t.Type == "Income")
        .Sum(t => t.Amount);

    public decimal TotalOutflows => Transactions
        .Where(t => t.Type == "Expense")
        .Sum(t => t.Amount);

    public decimal TotalDebt => Transactions
        .Where(t => t.Type == "Debt")
        .Sum(t => t.Amount);

    public decimal ClearedDebt => Transactions
        .Where(t => t.Type == "Debt Payment")
        .Sum(t => t.Amount);

    public decimal PendingDebt => TotalDebt - ClearedDebt;

    public int TotalTransactionsCount => Transactions.Count;

    public decimal TotalTransactionAmount => TotalInflows + TotalDebt - TotalOutflows;

    // Highest/Lowest Transactions
    public TransactionModel? HighestInflow => Transactions
        .Where(t => t.Type == "Income")
        .OrderByDescending(t => t.Amount)
        .FirstOrDefault();

    public TransactionModel? LowestInflow => Transactions
        .Where(t => t.Type == "Income")
        .OrderBy(t => t.Amount)
        .FirstOrDefault();

    public TransactionModel? HighestOutflow => Transactions
        .Where(t => t.Type == "Expense")
        .OrderByDescending(t => t.Amount)
        .FirstOrDefault();

    public TransactionModel? LowestOutflow => Transactions
        .Where(t => t.Type == "Expense")
        .OrderBy(t => t.Amount)
        .FirstOrDefault();

    public TransactionModel? HighestDebt => Transactions
        .Where(t => t.Type == "Debt")
        .OrderByDescending(t => t.Amount)
        .FirstOrDefault();

    public TransactionModel? LowestDebt => Transactions
        .Where(t => t.Type == "Debt")
        .OrderBy(t => t.Amount)
        .FirstOrDefault();

    // Methods
    public void AddTransaction(TransactionModel transaction)
    {
        Transactions.Add(transaction);
    }

    public void RemoveTransaction(TransactionModel transaction)
    {
        Transactions.Remove(transaction);
    }

    public decimal GetTotalBalance()
    {
        return TotalInflows - TotalOutflows;
    }

    // Filtering Method
    public List<TransactionModel> FilterTransactions(string type, List<string> tags, DateTime? startDate, DateTime? endDate)
    {
        return Transactions
            .Where(t => (string.IsNullOrEmpty(type) || t.Type == type) &&
                        (!tags.Any() || tags.All(tag => t.Tags.Contains(tag))) &&
                        (!startDate.HasValue || t.Date >= startDate) &&
                        (!endDate.HasValue || t.Date <= endDate))
            .ToList();
    }

    // Sorting Method
    public List<TransactionModel> SortTransactionsByDate(bool ascending = true)
    {
        return ascending ? Transactions.OrderBy(t => t.Date).ToList() : Transactions.OrderByDescending(t => t.Date).ToList();
    }

    // Searching Method
    public List<TransactionModel> SearchTransactions(string searchQuery)
    {
        return Transactions.Where(t => t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    // Top Transactions Method
    public List<TransactionModel> GetTopTransactions(int count = 5)
    {
        return Transactions.OrderByDescending(t => t.Amount).Take(count).ToList();
    }
    public void ClearDebt(decimal amountToClear)
    {
        var remainingAmount = amountToClear;

        foreach (var debt in Transactions.Where(t => t.Type == "Debt" && t.Amount > 0).OrderBy(t => t.Date))
        {
            if (remainingAmount <= 0)
                break;

            if (remainingAmount >= debt.Amount)
            {
                remainingAmount -= debt.Amount;
                debt.Amount = 0;
                Transactions.Add(new TransactionModel
                {
                    Description = $"Cleared Debt: {debt.Description}",
                    Amount = debt.Amount,
                    Date = DateTime.Now,
                    Type = "Debt Payment"
                });
            }
            else
            {
                debt.Amount -= remainingAmount;
                remainingAmount = 0;
            }
        }
    }
}
