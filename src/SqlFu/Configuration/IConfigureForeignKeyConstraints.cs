namespace SqlFu.Configuration
{
    public interface IConfigureForeignKeyConstraints
    {
        IConfigureForeignKeyConstraints OnUpdate(ForeignKeyRelationCascade option);
        IConfigureForeignKeyConstraints OnDelete(ForeignKeyRelationCascade option);
    }
}