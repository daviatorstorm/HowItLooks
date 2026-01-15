using HowItLooks.Models;
using SQLite;

namespace HowItLooks.Entities;

[Table("Campaigns")]
public class CampaignEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, NotNull]
    public string Name { get; set; } = string.Empty;

    public CampaignEntity() 
    {
    }
    public CampaignEntity(Campaign campaign)
    {
        Id = campaign.Id;
        Name = campaign.Name;
    }
}
