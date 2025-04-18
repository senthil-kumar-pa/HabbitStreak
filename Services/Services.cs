﻿using System.Text.Json;
using HabbitStreak.Models;

namespace HabbitStreak.Services
{
    public sealed class HabbitService
    {
        private static readonly Lazy<HabbitService> _instance = new Lazy<HabbitService>(() => new HabbitService());

        public static HabbitService Instance => _instance.Value;

        private readonly string _filePath;
        private List<Habbit>? _cachedHabbits;

        private HabbitService()
        {
            _filePath = "C:\\HabbitStreak\\Habbits.json";
        }

        public async Task<List<Habbit>> LoadHabbitsAsync()
        {
            if (_cachedHabbits != null)
                return _cachedHabbits;

            if (!File.Exists(_filePath))
                return new List<Habbit>();

            string json = await File.ReadAllTextAsync(_filePath);
            _cachedHabbits = JsonSerializer.Deserialize<List<Habbit>>(json) ?? new List<Habbit>();
            return _cachedHabbits;
        }

        public async Task SaveHabbitsAsync(List<Habbit> habbits)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(habbits, options);
            await File.WriteAllTextAsync(_filePath, json);

            // Invalidate cache
            _cachedHabbits = null;
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

        public async Task UpdateHabbitAsync(Habbit updatedHabbit, string newName, string newDescription, FrequencyType newFreqType, int freqCount, string newIcon)
        {
            var habbits = await LoadHabbitsAsync();
            var index = habbits.FindIndex(h => h.Id.Equals(updatedHabbit.Id));
            if (index >= 0)
            {
                updatedHabbit.SetNewName(newName);
                updatedHabbit.SetNewDescription(newDescription);
                updatedHabbit.SetFrequency(newFreqType, freqCount);
                updatedHabbit.SetNewIcon(newIcon);
                habbits[index] = updatedHabbit;
                await SaveHabbitsAsync(habbits);
            }
        }

        public async Task<bool> HabbitExistsAsync(string name)
        {
            var habbits = await LoadHabbitsAsync();
            return habbits.Any(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> HabbitExistsAsync(Guid id, string name)
        {
            var habbits = await LoadHabbitsAsync();
            return habbits.Any(h => h.Id != id && h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
