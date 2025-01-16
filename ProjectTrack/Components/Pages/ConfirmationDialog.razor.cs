using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProjectTrack.Components.Pages
{
    public partial class ConfirmationDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string ContentText { get; set; } = "Are you sure?";
        [Parameter] public string ButtonText { get; set; } = "Confirm";
        [Parameter] public string CancelText { get; set; } = "Cancel";

        private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
        private void Cancel() => MudDialog.Cancel();
    }
}