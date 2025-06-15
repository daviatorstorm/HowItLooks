namespace HowItLooks.Migrations;

public class _150620252240_AddArmorClass : BaseMigration
{
    public override string Name => nameof(_150620252240_AddArmorClass);

    public override int Version => 2;

    public override string GetSql()
    {
        return @"
                ALTER TABLE Enemies ADD COLUMN ArmorClass INTEGER;
            ";
    }
}
