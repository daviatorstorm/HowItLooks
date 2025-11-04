namespace HowItLooks.Migrations 
{
    public class _221020251201_AddGroups : BaseMigration
    {
        public override string Name => nameof(_221020251201_AddGroups);
        public override int Version => 2;

        public override List<string> GetSqlScripts()
        {
            return new List<string> 
            {
                @"
                    CREATE TABLE Groups (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                );",
                @"
                    ALTER TABLE Enemies ADD COLUMN GroupId INTEGER REFERENCES Groups(Id);
                "
            };
        }
    }                
}
