using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowItLooks.Migrations
{
    public class _060120261302_AddCampaigns : BaseMigration
    {
        public override string Name => nameof(_060120261302_AddCampaigns);
        public override int Version => 3;

        public override List<string> GetSqlScripts()
        {
            return new()
            {
                @"
                    CREATE TABLE Campaigns (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                );",
                @"
                    ALTER TABLE Enemies ADD COLUMN CampaignId INTEGER REFERENCES Campaigns(Id);
                "
            };
        }
    }
}
