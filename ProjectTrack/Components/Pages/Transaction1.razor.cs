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
        #region
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

            // Validate Debt Payment
            if (currentTransaction.Type == "Debt Payment")
            {
                // Check if there is any debt
                if (TransactionService.TotalDebt <= 0)
                {
                    Snackbar.Add("Error: There is no existing debt to make a debt payment!", Severity.Error);
                    return;
                }

                // Check if debt payment amount exceeds pending debt
                if (currentTransaction.Amount > TransactionService.PendingDebt)
                {
                    Snackbar.Add("Error: Debt payment amount cannot exceed the pending debt!", Severity.Error);
                    return;
                }

                // Check if debt payment amount exceeds inflows
                if (currentTransaction.Amount > TransactionService.TotalInflows)
                {
                    Snackbar.Add("Error: Debt payment amount cannot exceed the total inflows!", Severity.Error);
                    return;
                }
            }

            // Prevent saving if expense exceeds inflows
            if (currentTransaction.Type == "Expense")
            {
                var remainingInflows = TransactionService.TotalInflows - TransactionService.TotalOutflows;
                if (currentTransaction.Amount > remainingInflows)
                {
                    Snackbar.Add("Error: Expense exceeds the available inflows!", Severity.Error);
                    return;
                }
            }

            // Handle Tags
            if (!string.IsNullOrWhiteSpace(currentTransaction.SelectedTag))
            {
                currentTransaction.Tags = new List<string> { currentTransaction.SelectedTag };
            }

            // Save or update the transaction
            if (editingTransaction != null)
            {
                // Update the existing transaction
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
                TransactionService.AddTransaction(currentTransaction);

                // Warn if total outflows exceed total inflows
                if (TransactionService.TotalOutflows > TransactionService.TotalInflows)
                {
                    Snackbar.Add("Warning: Total outflows exceed total inflows!", Severity.Warning);
                }

                Snackbar.Add("New transaction added successfully!", Severity.Success);
            }

            // Reset the form after saving the transaction
            CancelTransactionForm();
        }

        #endregion



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
