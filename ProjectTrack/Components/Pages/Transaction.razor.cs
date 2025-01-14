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

        
        



        


    }
}