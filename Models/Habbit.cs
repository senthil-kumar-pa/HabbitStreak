namespace HabbitStreak.Models
{
    using System.Text.Json.Serialization;

    public class Habbit
    {
        public string Name { get; private set; }
        public DateTime CreatedDate { get; }
        public DateTime? LastCompletedDate { get; private set; }

        [JsonConstructor]
        public Habbit(string name, DateTime createdDate, DateTime? lastCompletedDate = null)
        {
            Name = name;
            CreatedDate = createdDate;
            LastCompletedDate = lastCompletedDate;
        }

        public void MarkTodayComplete()
        {
            LastCompletedDate = DateTime.Now;
        }

        public void SetNewName(string newName)
        {
            Name = newName;
        }
    }


}
