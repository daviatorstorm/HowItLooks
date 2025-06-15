namespace HowItLooks.Migrations;

public class _160620250039_AddCreatureType : BaseMigration
{
    public override string Name => nameof(_160620250039_AddCreatureType);

    public override int Version => 4;

    public override string GetSql()
    {
        return @"
            ALTER TABLE Enemies ADD COLUMN CreatureType INTEGER;
        ";
    }
}
