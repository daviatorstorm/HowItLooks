using HowItLooks.Models;
using SQLite;

namespace HowItLooks.Entities
{
    [Table("Groups")]
    public class GroupEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Name { get; set; } = string.Empty;

        public GroupEntity()
        {
        }
        public GroupEntity(Group group)
        {
            Id = group.Id;
            Name = group.Name;
        }
    }
}