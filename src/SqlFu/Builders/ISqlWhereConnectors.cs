namespace SqlFu.Builders
{
    public interface ISqlWhereConnectors<T, TProjection>:ISqlWhere<T,TProjection>
    {
        ISqlWhere<T, TProjection> And { get; }
        ISqlWhere<T, TProjection> Or { get; }        
    }
}