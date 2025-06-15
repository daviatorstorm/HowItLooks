namespace HowItLooks.Migrations;

public class _160620250021_AddTempHP : BaseMigration
{
    public override string Name => nameof(_160620250021_AddTempHP);

    public override int Version => 3;

    public override string GetSql()
    {
        return @"
            ALTER TABLE Enemies ADD COLUMN TempHitPoints INTEGER;
        ";
    }
}
