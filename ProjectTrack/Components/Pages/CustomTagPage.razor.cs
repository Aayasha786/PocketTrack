using MudBlazor;

namespace ProjectTrack.Components.Pages
{
    public partial class CustomTagPage
    {
        private string newTag = "";
        private List<TagModel> Tags { get; set; } = new List<TagModel>();

        protected override void OnInitialized()
        {
            // Load Tags on Page Initialization
            Tags = TagService.GetAllTags();
        }

        private void AddTag()
        {
            if (!string.IsNullOrWhiteSpace(newTag) && !Tags.Any(t => t.Name == newTag))
            {
                Tags.Add(new TagModel { Name = newTag });
                TagService.SaveTags(Tags); // Persist to file or database
                newTag = ""; // Clear the input
                Snackbar.Add("Tag added successfully!", Severity.Success); // Show success message
            }
            else if (!string.IsNullOrWhiteSpace(newTag) && Tags.Any(t => t.Name == newTag))
            {
                Snackbar.Add("Tag already exists!", Severity.Warning); // Show warning if the tag already exists
            }
            else
            {
                Snackbar.Add("Tag name cannot be empty!", Severity.Error); // Show error if the input is empty
            }
        }

        private void DeleteTag(TagModel tag)
        {
            Tags.Remove(tag);
            TagService.SaveTags(Tags); // Persist changes
            Snackbar.Add("Tag deleted successfully!", Severity.Info); // Show delete success message
        }
    }
}