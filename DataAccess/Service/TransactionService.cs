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
    public event Action OnTransactionsChanged;

    private void NotifyTransactionsChanged()
    {
        OnTransactionsChanged?.Invoke();
    }

    public string AddTransaction(TransactionModel transaction)
    {
        if (transaction.Type == "Debt" && string.IsNullOrEmpty(transaction.DebtId))
        {
            transaction.DebtId = Guid.NewGuid().ToString(); // Generate unique DebtId
        }

        if (transaction.Type == "Expense")
        {
            var availableIncome = TotalInflows - TotalOutflows;

            if (transaction.Amount > availableIncome)
            {
                return "Error: Expense exceeds available income!";
            }

            DeductIncome(transaction.Amount);
        }

        Transactions.Add(transaction);
        SaveTransactions();
        NotifyTransactionsChanged(); // Notify UI about changes
        return "Transaction added successfully!";
    }



    // Deduct Income for Expenses
    private void DeductIncome(decimal expenseAmount)
    {
        var remainingExpense = expenseAmount;

        foreach (var income in Transactions
            .Where(t => t.Type == "Income" && t.Amount > 0)
            .OrderBy(t => t.Date))
        {
            if (remainingExpense <= income.Amount)
            {
                income.Amount -= remainingExpense;
                break;
            }
            else
            {
                remainingExpense -= income.Amount;
                income.Amount = 0;
            }
        }
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

    // Other Methods (Filtering, Sorting, Searching, etc.)
    public List<TransactionModel> FilterTransactions(string type, List<string> tags, DateTime? startDate, DateTime? endDate)
    {
        return Transactions
            .Where(t => (string.IsNullOrEmpty(type) || t.Type == type) &&
                        (!tags.Any() || tags.All(tag => t.Tags.Contains(tag))) &&
                        (!startDate.HasValue || t.Date >= startDate) &&
                        (!endDate.HasValue || t.Date <= endDate))
            .ToList();
    }

    public List<TransactionModel> SortTransactionsByDate(bool ascending = true)
    {
        return ascending ? Transactions.OrderBy(t => t.Date).ToList() : Transactions.OrderByDescending(t => t.Date).ToList();
    }

    public List<TransactionModel> SearchTransactions(string searchQuery)
    {
        return Transactions
            .Where(t => t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<TransactionModel> GetTopTransactions(int count = 5)
    {
        return Transactions
            .OrderByDescending(t => t.Amount)
            .Take(count)
            .ToList();
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

                // Create a debt payment transaction
                Transactions.Add(new TransactionModel
                {
                    Description = $"Cleared Debt: {debt.Description}",
                    Amount = debt.Amount,
                    Date = DateTime.Now,
                    Type = "Debt Payment"
                });

                debt.Amount = 0; // Fully cleared
            }
            else
            {
                debt.Amount -= remainingAmount;

                // Create a partial debt payment transaction
                Transactions.Add(new TransactionModel
                {
                    Description = $"Partially Cleared Debt: {debt.Description}",
                    Amount = remainingAmount,
                    Date = DateTime.Now,
                    Type = "Debt Payment"
                });

                remainingAmount = 0;
            }
        }

        SaveTransactions();
    }

    public List<TransactionModel> GetPendingDebtDetails()
    {
        var debts = Transactions.Where(t => t.Type == "Debt").ToList();

        return debts.Select(debt =>
        {
            // Match DebtId with Debt Payment transactions
            var clearedAmount = Transactions
                .Where(payment => payment.Type == "Debt Payment" && payment.DebtId == debt.DebtId)
                .Sum(payment => payment.Amount);

            var pendingAmount = debt.Amount - clearedAmount;

            return new TransactionModel
            {
                Description = debt.Description,
                Amount = Math.Max(pendingAmount, 0), // Ensure non-negative pending amount
                Date = debt.Date,
                DebtSource = debt.DebtSource,
                DueDate = debt.DueDate,
                Note = debt.Note,
                DebtId = debt.DebtId
            };
        }).Where(debt => debt.Amount > 0).ToList();
    }



}
