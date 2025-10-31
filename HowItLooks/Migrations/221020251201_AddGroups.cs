namespace HowItLooks.Migrations;

public class _221020251201_AddGroups : BaseMigration
{
    public override string Name => nameof(_221020251201_AddGroups);
    public override int Version => 5;

    public override string GetSql()
    {
        return @"
                CREATE TABLE Groups (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );

                ALTER TABLE Enemies ADD COLUMN GroupId INTEGER REFERENCES Groups(Id);
        ";
    }
}
