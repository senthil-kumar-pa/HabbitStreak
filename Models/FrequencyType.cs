using System.Text.Json.Serialization;

namespace HabbitStreak.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FrequencyType
    {
        Daily,
        Weekly,
        Monthly
    }
}
