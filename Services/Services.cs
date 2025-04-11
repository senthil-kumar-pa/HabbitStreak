/*using System.Text.Json;
using HabbitStreak.Models;

namespace HabbitStreak.Services
{
    public partial class HabbitService
    {
        private static string GetFilePath()
        {
            return "C:\\HabbitStreak\\Habbits.json";
        }

        public static async Task<List<Habbit>> LoadHabbitsAsync()
        {
            if (!File.Exists(GetFilePath())) return [];
            string json = await File.ReadAllTextAsync(GetFilePath());
            return JsonSerializer.Deserialize<List<Habbit>>(json) ?? [];
        }

        public static void SaveHabbits(List<Habbit> Habbits)
        {
            string json = JsonSerializer.Serialize(Habbits);
            File.WriteAllText(GetFilePath(), json);
        }

        public static async Task UpdateHabbitAsync(Habbit updatedHabbit)
        {
            var habbits = await LoadHabbitsAsync();
            var index = habbits.FindIndex(h => h.Name == updatedHabbit.Name);
            if (index >= 0)
            {
                habbits[index] = updatedHabbit;
                SaveHabbits(habbits);
            }
        }

        public static async Task UpdateHabbitAsync(Habbit updatedHabbit, string newName)
        {
            var habbits = await LoadHabbitsAsync();
            var index = habbits.FindIndex(h => h.Name.Equals(updatedHabbit.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                updatedHabbit.SetNewName(newName);
                habbits[index] = updatedHabbit;
                SaveHabbits(habbits);
            }
        }

        public static async Task<bool> HabbitExistsAsync(string name)
        {
            var habbits = await LoadHabbitsAsync();
            return habbits.Any(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
*/

using System.Text.Json;
using HabbitStreak.Models;

namespace HabbitStreak.Services
{
    public sealed class HabbitService
    {
        private static readonly Lazy<HabbitService> _instance = new Lazy<HabbitService>(() => new HabbitService());

        public static HabbitService Instance => _instance.Value;

        private readonly string _filePath;

        private HabbitService()
        {
            _filePath = "C:\\HabbitStreak\\Habbits.json";
        }

        public async Task<List<Habbit>> LoadHabbitsAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Habbit>();

            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Habbit>>(json) ?? new List<Habbit>();
        }

        public async Task SaveHabbitsAsync(List<Habbit> habbits)
        {
            string json = JsonSerializer.Serialize(habbits);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task UpdateHabbitAsync(Habbit updatedHabbit)
        {
            var habbits = await LoadHabbitsAsync();
            var index = habbits.FindIndex(h => h.Name == updatedHabbit.Name);
            if (index >= 0)
            {
                habbits[index] = updatedHabbit;
                await SaveHabbitsAsync(habbits);
            }
        }

        public async Task UpdateHabbitAsync(Habbit updatedHabbit, string newName)
        {
            var habbits = await LoadHabbitsAsync();
            var index = habbits.FindIndex(h => h.Name.Equals(updatedHabbit.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                updatedHabbit.SetNewName(newName);
                habbits[index] = updatedHabbit;
                await SaveHabbitsAsync(habbits);
            }
        }

        public async Task<bool> HabbitExistsAsync(string name)
        {
            var habbits = await LoadHabbitsAsync();
            return habbits.Any(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
