namespace HowItLooks.Migrations
{
    public class _150620251724_Initial : BaseMigration
    {
        public override string Name => nameof(_150620251724_Initial);

        public override int Version => 1;

        public override List<string> GetSqlScripts()
        {
            return new List<string>
            {
                @"
                    CREATE TABLE Enemies (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        HitPoints INTEGER NOT NULL,
                        HitPointsLeft INTEGER NOT NULL,
                        Initiative INTEGER NOT NULL,
                        IsActive BOOLEAN NOT NULL DEFAULT 1
                );",
                @"
                    ALTER TABLE Enemies ADD COLUMN ArmorClass INTEGER;
                ",
                @"
                    ALTER TABLE Enemies ADD COLUMN TempHitPoints INTEGER;
                ",
                @"
                    ALTER TABLE Enemies ADD COLUMN CreatureType INTEGER;
                "
            };
        }
    }
}
