using MudBlazor;
using ProjectTrack.Models;

namespace ProjectTrack.Components.Pages
{
    public partial class Transaction
    {

        private string FilterType { get; set; } = "";
        private string SearchQuery { get; set; } = "";
        private string SortOrder { get; set; } = "Ascending";
        private DateTime? StartDate { get; set; }
        private DateTime? EndDate { get; set; }

        protected TransactionModel? editingTransaction { get; set; }
        protected bool showAddTransactionForm { get; set; } = false;

        private List<TagModel> AvailableTags { get; set; } = new();
        protected TransactionModel currentTransaction { get; set; } = new() { Date = DateTime.Now, Type = "Income", Tags = new List<string>() };
        protected List<TransactionModel> Transactions => TransactionService.Transactions;

        private List<TransactionModel> FilteredTransactions => ApplySorting(ApplySearch(ApplyDateFilter(ApplyFilter(Transactions))));


       
        
        protected override void OnInitialized()
        {
            
            TransactionService.OnTransactionsChanged += StateHasChanged; 
            AvailableTags = TagService.GetAllTags();
        }
        public void Dispose()
        {
            TransactionService.OnTransactionsChanged -= StateHasChanged; // Unsubscribe to avoid memory leaks
        }


        protected void ApplyFilters()
        {
            StateHasChanged();
        }

        private List<TransactionModel> ApplyFilter(List<TransactionModel> transactions)
        {
            return string.IsNullOrEmpty(FilterType) ? transactions : transactions.Where(t => t.Type == FilterType).ToList();
        }

        private List<TransactionModel> ApplySearch(List<TransactionModel> transactions)
        {
            return string.IsNullOrEmpty(SearchQuery)
                ? transactions
                : transactions.Where(t => t.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private List<TransactionModel> ApplyDateFilter(List<TransactionModel> transactions)
        {
            return transactions
                .Where(t => !StartDate.HasValue || t.Date >= StartDate.Value)
                .Where(t => !EndDate.HasValue || t.Date <= EndDate.Value)
                .ToList();
        }

        private List<TransactionModel> ApplySorting(List<TransactionModel> transactions)
        {
            return SortOrder == "Ascending"
                ? transactions.OrderBy(t => t.Date).ToList()
                : transactions.OrderByDescending(t => t.Date).ToList();
        }

        protected void SaveTransaction()
        {
            // Validation for mandatory fields
            if (string.IsNullOrWhiteSpace(currentTransaction.Description))
            {
                Snackbar.Add("Error: Description is required!", Severity.Error);
                return;
            }

            if (currentTransaction.Amount <= 0)
            {
                Snackbar.Add("Error: Amount must be greater than zero!", Severity.Error);
                return;
            }

            if (currentTransaction.Date == default)
            {
                Snackbar.Add("Error: Date is required!", Severity.Error);
                return;
            }

            // Validate Source for Debt and Debt Payment
            if ((currentTransaction.Type == "Debt" || currentTransaction.Type == "Debt Payment") &&
                string.IsNullOrWhiteSpace(currentTransaction.DebtSource))
            {
                Snackbar.Add("Error: Source is required for Debt and Debt Payment transactions!", Severity.Error);
                return;
            }

            // Link DebtId for Debt Payment
            if (currentTransaction.Type == "Debt Payment" && string.IsNullOrEmpty(currentTransaction.DebtId))
            {
                Snackbar.Add("Error: Debt Payment must be linked to a specific Debt!", Severity.Error);
                return;
            }

            // Handle Tags
            if (!string.IsNullOrWhiteSpace(currentTransaction.SelectedTag))
            {
                currentTransaction.Tags = new List<string> { currentTransaction.SelectedTag };
            }

            // Save or update the transaction
            if (editingTransaction != null)
            {
                editingTransaction.Description = currentTransaction.Description;
                editingTransaction.Amount = currentTransaction.Amount;
                editingTransaction.Date = currentTransaction.Date;
                editingTransaction.Type = currentTransaction.Type;
                editingTransaction.Tags = currentTransaction.Tags;
                editingTransaction.DebtSource = currentTransaction.DebtSource;
                editingTransaction.DueDate = currentTransaction.DueDate;
                editingTransaction.DebtId = currentTransaction.DebtId; // Link DebtId

                Snackbar.Add("Transaction updated successfully!", Severity.Success);
            }
            else
            {
                var result = TransactionService.AddTransaction(currentTransaction); // Only call AddTransaction here

                if (result.StartsWith("Error"))
                {
                    Snackbar.Add(result, Severity.Error); // Show error message if validation fails
                    return;
                }

                Snackbar.Add("New transaction added successfully!", Severity.Success);
            }

            CancelTransactionForm();
        }


        protected void ToggleAddTransactionForm()
        {
            showAddTransactionForm = !showAddTransactionForm;
            if (!showAddTransactionForm)
            {
                CancelTransactionForm();
            }
        }

        protected void EditTransaction(TransactionModel transaction)
        {
            editingTransaction = transaction;
            currentTransaction = new TransactionModel
            {
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type,
                Tags = new List<string>(transaction.Tags),
                Note = transaction.Note
            };
            showAddTransactionForm = true;
        }

        protected void CancelTransactionForm()
        {
            currentTransaction = new TransactionModel
            {
                Date = DateTime.Now,
                Type = "Income",
                Tags = new List<string>()
            };
            editingTransaction = null;
            showAddTransactionForm = false;
        }

        protected void DeleteTransaction(TransactionModel transaction)
        {
            TransactionService.RemoveTransaction(transaction);
            Snackbar.Add("Transaction deleted successfully!", Severity.Info);
        }
    }
}
