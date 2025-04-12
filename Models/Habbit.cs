namespace HabbitStreak.Models
{
    using System.Text.Json.Serialization;

    [method: JsonConstructor]
    public class Habbit(Guid id, string name, string description, DateTime createdDate, DateTime? lastCompletedDate = null)
    {
        public Guid Id { get; private set; } = id;
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateTime CreatedDate { get; } = createdDate;
        public DateTime? LastCompletedDate { get; private set; } = lastCompletedDate;

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
    }
}
