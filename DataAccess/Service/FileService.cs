using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectTrack.Services
{
    public class FileService
    {
        // Save data to a file
        public async Task SaveToFileAsync<T>(string filePath, T data)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);

                // Ensure the directory exists
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        // Load data from a file
        public async Task<T?> LoadFromFileAsync<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }

            return default;
        }
    }
}
