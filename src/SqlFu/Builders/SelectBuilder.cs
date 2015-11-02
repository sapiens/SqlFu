namespace SqlFu.Builders
{
    //public class SelectBuilder<T>
    //{
    //    public SelectBuilder()
    //    {
    //        IWhere<IdName> f;

    //        IFrom db;

    //        //IBuildSql sub = db.From<IdName>().SelectAll();

    //        //f.Where(t=>t.Id).In(db.From<>());
    //    }


    //}

    //public interface IFrom
    //{
    //    ISelect<T> From<T>();
    //}

    //public interface ISelect<T>:IBuildSql
    //{
    //    void Select<TProjection>(Expression<Func<T, TProjection>> projection);
    //    IBuildSql SelectAll();
    //    void Select<TFirstJoin,TProjection>(Expression<Func<T,TFirstJoin, TProjection>> projection);
    //    void Select<TFirstJoin,TSecondJoin,TProjection>(Expression<Func<T,TFirstJoin,TSecondJoin, TProjection>> projection);
    //}

    //public interface IJoinTables<T>
    //{
    //    void InnerJoin<TOther>(Expression<Func<T, TOther, bool>> joinCondition);
    //    void InnerJoin<TOther,TOther2>(Expression<Func<TOther, TOther2, bool>> joinCondition);
    //    void LeftJoin<TOther>(Expression<Func<T, TOther, bool>> joinCondition);
    //    void LeftJoin<TOther, TOther2>(Expression<Func<TOther, TOther2, bool>> joinCondition);
    //}

    //public interface IWhere<T>
    //{
    //    void Where(Expression<Func<T, bool>> condition);
    //    ISubquery Where(Expression<Func<T, object>> column);
    //}

    //public interface IBuildSql<TProjection>
    //{
    //    string Build();

    //}

    //public interface ISubquery
    //{
    //    void In(IBuildSql subquery);
    //    void Equals(IBuildSql subquery);
    //}

    ////public interface IGroupBy<T>
    ////{
    ////    IHaving GroupBy(params Expression<Func<T, object>>[] columns);
    ////}

    //public interface IHaving<T,TProjection>
    //{
    //    IOrderBy<T, TProjection> Having(Expression<Func<T,bool>> condition);
    //}

    //public interface IOrderBy<T, TProjection>
    //{
    //    IBuildSql OrderBy(Expression<Func<T, TProjection, object>> column);
    //    IBuildSql OrderByDescending(Expression<Func<T, TProjection, object>> column);
    //}

    //public interface IThenOrderBy<T, TProjection>
    //{
    //    IOrderBy<T, TProjection> Then { get; }
    //}

    //public interface ILimitQuery
    //{
    //    IBuildSql Limit(long skip, int take);
    //}
}