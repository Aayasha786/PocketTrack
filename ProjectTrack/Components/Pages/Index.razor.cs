using Microsoft.AspNetCore.Components;

namespace ProjectTrack.Components.Pages
{
    public partial class Index : ComponentBase
    {
        protected override void OnInitialized()
        {
            Nav.NavigateTo("/home");
        }

    }
}