namespace HabbitStreak.Models
{
    using System;
    using System.Text.Json.Serialization;

    [method: JsonConstructor]
    public class Habbit(
        Guid id,
        string icon,
        string name,
        string description,
        DateTime createdDate,
        DateTime? lastCompletedDate = null,
        FrequencyType frequency = FrequencyType.Daily,
        int frequencyCount = 1)
    {
        public Guid Id { get; private set; } = id;
        public string Icon { get; private set; } = icon;


        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateTime CreatedDate { get; } = createdDate;
        public DateTime? LastCompletedDate { get; private set; } = lastCompletedDate;
        public FrequencyType Frequency { get; private set; } = frequency;
        public int FrequencyCount { get; private set; } = frequencyCount;

        public void MarkTodayComplete()
        {
            LastCompletedDate = DateTime.Now;
        }

        public void SetNewName(string newName)
        {
            Name = newName;
        }

        public void SetNewDescription(string newDescription)
        {
            Description = newDescription;
        }
        public void SetNewIcon(string newIcon)
        {
            Icon = newIcon;
        }
        public void SetFrequency(FrequencyType newFrequency, int count = 1)
        {
            Frequency = newFrequency;
            FrequencyCount = count;
        }

        [JsonIgnore]
        public string FrequencyDisplay =>
            Frequency switch
            {
                FrequencyType.Daily => "Daily",
                FrequencyType.Weekly => $"Weekly: {FrequencyCount}x",
                FrequencyType.Monthly => $"Monthly: {FrequencyCount}x",
                _ => "Unknown"
            };

        [JsonIgnore]
        public string DisplayIcon =>
            !string.IsNullOrEmpty(Icon) ? Icon.Split("-")[0].Trim() : "🔥";
    }
}
