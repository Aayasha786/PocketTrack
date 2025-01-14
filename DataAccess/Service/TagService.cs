using System.Text.Json;

public class TagService
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tags.json");

    private List<TagModel> Tags { get; set; }

    // Constructor to load tags on initialization
    public TagService()
    {
        Tags = LoadTags();
    }

    // Load all tags from the file or return default tags if the file does not exist
    private List<TagModel> LoadTags()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            var loadedTags = JsonSerializer.Deserialize<List<TagModel>>(json);
            return loadedTags ?? GetDefaultTags();
        }
        else
        {
            // Save default tags to the file if it does not exist
            var defaultTags = GetDefaultTags();
            SaveTags(defaultTags);
            return defaultTags;
        }
    }

    // Get all tags (called externally)
    public List<TagModel> GetAllTags()
    {
        return Tags;
    }

    // Add a new tag and save it to the file
    public void AddTag(string tagName)
    {
        if (!string.IsNullOrWhiteSpace(tagName) && !Tags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
        {
            Tags.Add(new TagModel { Name = tagName });
            SaveTags(Tags);
        }
    }

    // Remove a tag by name and save the changes
    public void DeleteTag(string tagName)
    {
        var tagToRemove = Tags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tagToRemove != null)
        {
            Tags.Remove(tagToRemove);
            SaveTags(Tags);
        }
    }

    // Save tags to the JSON file
    public void SaveTags(List<TagModel> tags)
    {
        var json = JsonSerializer.Serialize(tags);
        File.WriteAllText(FilePath, json);
    }

    // Return a list of default tags
    private List<TagModel> GetDefaultTags()
    {
        return new List<TagModel>
        {
            new TagModel { Name = "Yearly" },
            new TagModel { Name = "Monthly" },
            new TagModel { Name = "Food" },
            new TagModel { Name = "Drinks" },
            new TagModel { Name = "Clothes" },
            new TagModel { Name = "Gadgets" },
            new TagModel { Name = "Miscellaneous" },
            new TagModel { Name = "Fuel" },
            new TagModel { Name = "Rent" },
            new TagModel { Name = "EMI" },
            new TagModel { Name = "Party" }
        };
    }
}
