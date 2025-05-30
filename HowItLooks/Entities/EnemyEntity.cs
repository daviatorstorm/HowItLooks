using HowItLooks.Models;
using SQLite;

namespace HowItLooks.Entities;

public class EnemyEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int HitPoints { get; set; }
    public int HitPointsLeft { get; set; }
    public int Initiative { get; set; }

    public bool IsActive { get; set; }

    public EnemyEntity()
    {
    }

    public EnemyEntity(Enemy enemy)
    {
        Id = enemy.Id;
        Name = enemy.Name;
        HitPoints = enemy.HitPoints;
        HitPointsLeft = enemy.HitPointsLeft;
        Initiative = enemy.Initiative;
        IsActive = enemy.IsActive;
    }
}
