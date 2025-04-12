namespace HabbitStreak.Services
{
    using System.Text.Json;
    using HabbitStreak.Models;

    public class SettingsService
    {
        private readonly string _filePath;

        public SettingsService()
        {
            _filePath = "C:\\HabbitStreak\\Settings.json";
        }

        public async Task<SettingsModel> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new SettingsModel();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
        }

        public async Task SaveAsync(SettingsModel settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }

}
