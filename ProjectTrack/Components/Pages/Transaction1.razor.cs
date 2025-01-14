using MudBlazor;
using ProjectTrack.Models;

namespace ProjectTrack.Components.Pages
{
    public partial class Transaction
    {
        private string FilterType { get; set; } = "";
        private string SearchQuery { get; set; } = "";
        private string SortOrder { get; set; } = "Ascending";
        private string currentTagsInput { get; set; } = "";
        private DateTime? StartDate { get; set; }
        private DateTime? EndDate { get; set; }

        private List<TransactionModel> FilteredTransactions => ApplySorting(ApplySearch(ApplyDateFilter(ApplyFilter(Transactions))));

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

        private void SaveTransaction()
        {
            if (!string.IsNullOrEmpty(currentTagsInput))
            {
                currentTransaction.Tags = currentTagsInput.Split(",")
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .ToList();
            }

            if (editingTransaction != null)
            {
                editingTransaction.Description = currentTransaction.Description;
                editingTransaction.Amount = currentTransaction.Amount;
                editingTransaction.Date = currentTransaction.Date;
                editingTransaction.Type = currentTransaction.Type;
                editingTransaction.Tags = currentTransaction.Tags;
                editingTransaction.Note = currentTransaction.Note;
            }
            else
            {
                Transactions.Add(new TransactionModel
                {
                    Description = currentTransaction.Description,
                    Amount = currentTransaction.Amount,
                    Date = currentTransaction.Date,
                    Type = currentTransaction.Type,
                    Tags = currentTransaction.Tags,
                    Note = currentTransaction.Note
                });
            }

            CancelTransactionForm();
        }

        protected void CancelTransactionForm()
        {
            currentTransaction = new TransactionModel
            {
                Date = DateTime.Now,
                Type = "Income"
            };
            currentTagsInput = "";
            editingTransaction = null;
            showAddTransactionForm = false;
        }

    }
}