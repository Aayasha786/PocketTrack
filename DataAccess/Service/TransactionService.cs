using System.Text.Json;
using ProjectTrack.Models;

public class TransactionService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "transactions.json"
    );

    public List<TransactionModel> Transactions { get; private set; } = new();

    // Constructor
    public TransactionService()
    {
        LoadTransactions();
    }

    // Load Transactions from File
    private void LoadTransactions()
    {
        if (!File.Exists(FilePath))
        {
            Transactions = new List<TransactionModel>();
            return;
        }

        var json = File.ReadAllText(FilePath);
        Transactions = JsonSerializer.Deserialize<List<TransactionModel>>(json) ?? new List<TransactionModel>();
    }

    // Save Transactions to File
    private void SaveTransactions()
    {
        var json = JsonSerializer.Serialize(Transactions);
        File.WriteAllText(FilePath, json);
    }

    // Add Transaction
    public void AddTransaction(TransactionModel transaction)
    {
        Transactions.Add(transaction);
        SaveTransactions();
    }

    // Remove Transaction
    public void RemoveTransaction(TransactionModel transaction)
    {
        Transactions.Remove(transaction);
        SaveTransactions();
    }

    // Properties for Financial Summaries
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

    // Filtering Transactions
    public List<TransactionModel> FilterTransactions(string type, List<string> tags, DateTime? startDate, DateTime? endDate)
    {
        return Transactions
            .Where(t => (string.IsNullOrEmpty(type) || t.Type == type) &&
                        (!tags.Any() || tags.All(tag => t.Tags.Contains(tag))) &&
                        (!startDate.HasValue || t.Date >= startDate) &&
                        (!endDate.HasValue || t.Date <= endDate))
            .ToList();
    }

    // Sorting Transactions by Date
    public List<TransactionModel> SortTransactionsByDate(bool ascending = true)
    {
        return ascending ? Transactions.OrderBy(t => t.Date).ToList() : Transactions.OrderByDescending(t => t.Date).ToList();
    }

    // Searching Transactions
    public List<TransactionModel> SearchTransactions(string searchQuery)
    {
        return Transactions
            .Where(t => t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Getting Top Transactions by Amount
    public List<TransactionModel> GetTopTransactions(int count = 5)
    {
        return Transactions
            .OrderByDescending(t => t.Amount)
            .Take(count)
            .ToList();
    }

    // Clearing Debt Logic
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

        SaveTransactions(); // Save updated transaction list
    }
}
