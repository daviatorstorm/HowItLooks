namespace HowItLooks.Migrations
{
    public abstract class BaseMigration
    {
        public abstract List<string> GetSqlScripts();
        public abstract string Name { get; }
        public abstract int Version { get; }
    }
}
