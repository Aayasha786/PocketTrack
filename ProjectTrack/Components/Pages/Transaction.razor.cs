using ProjectTrack.Models;
using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace ProjectTrack.Components.Pages
{
    public partial class TransactionsBase : ComponentBase
    {
        [Inject] public TransactionService TransactionService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        protected List<TransactionModel> Transactions => TransactionService.Transactions;
        protected TransactionModel currentTransaction { get; set; } = new TransactionModel { Date = DateTime.Now, Type = "Income" };
        protected TransactionModel? editingTransaction { get; set; }
        protected bool showAddTransactionForm { get; set; } = false;

        protected void ToggleAddTransactionForm()
        {
            showAddTransactionForm = !showAddTransactionForm;
            if (!showAddTransactionForm)
            {
                currentTransaction = new TransactionModel { Date = DateTime.Now, Type = "Income" };
                editingTransaction = null;
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
                Type = transaction.Type
            };
            showAddTransactionForm = true;
        }

        protected void DeleteTransaction(TransactionModel transaction)
        {
            TransactionService.RemoveTransaction(transaction);
            Snackbar.Add("Transaction deleted successfully!", Severity.Info);
        }



        protected void SaveTransaction()
        {
            if (currentTransaction.Type == "Expense" && currentTransaction.Amount > TransactionService.TotalInflows)
            {
                Snackbar.Add("Error: Expense exceeds total available income!", Severity.Error);
                return;
            }

            if (editingTransaction != null)
            {
                editingTransaction.Description = currentTransaction.Description;
                editingTransaction.Amount = currentTransaction.Amount;
                editingTransaction.Date = currentTransaction.Date;
                editingTransaction.Type = currentTransaction.Type;

                if (currentTransaction.Type == "Expense")
                {
                    DeductIncome(currentTransaction.Amount);
                }

                Snackbar.Add("Transaction updated successfully!", Severity.Success);
            }
            else
            {
                if (currentTransaction.Type == "Expense")
                {
                    DeductIncome(currentTransaction.Amount);
                }

                TransactionService.AddTransaction(currentTransaction);
                Snackbar.Add("New transaction added successfully!", Severity.Success);
            }

            ToggleAddTransactionForm();
        }

        protected void DeductIncome(decimal expenseAmount)
        {
            var remainingExpense = expenseAmount;

            foreach (var income in TransactionService.Transactions
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



    }
}