using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectTrack.Components.Pages
{
    public partial class TransactionsBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }

        public class TransactionModel
        {
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string Type { get; set; } // Income, Expense, Debt, or Debt Payment
        }

        protected List<TransactionModel> Transactions = new();
        protected TransactionModel? currentTransaction = null;
        protected TransactionModel? editingTransaction = null;
        protected bool showAddTransactionForm = false;

        protected decimal TotalInflows => Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
        protected decimal TotalOutflows => Transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        protected decimal TotalDebts => Transactions.Where(t => t.Type == "Debt").Sum(t => t.Amount);
        protected decimal ClearedDebt => Transactions.Where(t => t.Type == "Debt Payment").Sum(t => t.Amount);
        protected decimal RemainingDebt => TotalDebts - ClearedDebt;

        protected TransactionModel? HighestInflow => Transactions
             .Where(t => t.Type == "Income")
             .OrderByDescending(t => t.Amount)
             .FirstOrDefault();

        protected TransactionModel? LowestInflow => Transactions
            .Where(t => t.Type == "Income")
            .OrderBy(t => t.Amount)
            .FirstOrDefault();

        protected TransactionModel? HighestOutflow => Transactions
            .Where(t => t.Type == "Expense")
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();

        protected TransactionModel? LowestOutflow => Transactions
            .Where(t => t.Type == "Expense")
            .OrderBy(t => t.Amount)
            .FirstOrDefault();

        protected TransactionModel? HighestDebt => Transactions
            .Where(t => t.Type == "Debt")
            .OrderByDescending(t => t.Amount)
            .FirstOrDefault();

        protected TransactionModel? LowestDebt => Transactions
            .Where(t => t.Type == "Debt")
            .OrderBy(t => t.Amount)
            .FirstOrDefault();
        protected void ToggleAddTransactionForm()
        {
            showAddTransactionForm = true;
            currentTransaction = new TransactionModel { Date = DateTime.Now, Type = "Income" };
        }

        protected void EditTransaction(TransactionModel transaction)
        {
            editingTransaction = transaction;
            currentTransaction = new TransactionModel
            {
                Description = transaction.Description,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Type = transaction.Type
            };
        }

        protected void SaveTransaction()
        {
            if (currentTransaction == null) return;

            if (!string.IsNullOrWhiteSpace(currentTransaction.Description) && currentTransaction.Amount > 0)
            {
                if (editingTransaction != null)
                {
                    editingTransaction.Description = currentTransaction.Description;
                    editingTransaction.Date = currentTransaction.Date;
                    editingTransaction.Amount = currentTransaction.Amount;
                    editingTransaction.Type = currentTransaction.Type;
                    Snackbar.Add("Transaction updated successfully!", Severity.Success);
                }
                else
                {
                    if (currentTransaction.Type == "Expense")
                    {
                        HandleExpense(currentTransaction);
                    }
                    else
                    {
                        Transactions.Add(new TransactionModel
                        {
                            Description = currentTransaction.Description,
                            Date = currentTransaction.Date,
                            Amount = currentTransaction.Amount,
                            Type = currentTransaction.Type
                        });
                    }

                    if (TotalOutflows > TotalInflows)
                    {
                        Snackbar.Add("Warning: Total outflows exceed total inflows!", Severity.Warning);
                    }

                    Snackbar.Add("New transaction added successfully!", Severity.Success);
                }

                CancelTransactionForm();
            }
        }

        private void HandleExpense(TransactionModel expense)
        {
            // Calculate remaining expense to deduct from inflows
            decimal remainingExpense = expense.Amount;

            // Iterate through inflows in chronological order
            foreach (var inflow in Transactions
                .Where(t => t.Type == "Income" && t.Amount > 0)
                .OrderBy(t => t.Date))
            {
                if (remainingExpense <= inflow.Amount)
                {
                    inflow.Amount -= remainingExpense;
                    remainingExpense = 0;
                    break;
                }
                else
                {
                    remainingExpense -= inflow.Amount;
                    inflow.Amount = 0;
                }
            }

            if (remainingExpense > 0)
            {
                Snackbar.Add("Warning: Not enough inflows to cover this expense. Some inflows are insufficient.", Severity.Warning);
            }

            // Add the expense to the transaction list
            Transactions.Add(new TransactionModel
            {
                Description = expense.Description,
                Date = expense.Date,
                Amount = expense.Amount,
                Type = "Expense"
            });
        }

        protected void CancelTransactionForm()
        {
            currentTransaction = null;
            editingTransaction = null;
            showAddTransactionForm = false;
        }

        protected void DeleteTransaction(TransactionModel transaction)
        {
            Transactions.Remove(transaction);
            Snackbar.Add("Transaction deleted successfully.", Severity.Info);
        }
    }
}
