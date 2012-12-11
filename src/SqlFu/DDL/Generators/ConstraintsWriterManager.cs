using System.Collections.Generic;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    //internal class ConstraintsWriterManager:AbstractSchemaItemWriter
    //{
    //    private ConstraintsCollection _constraints;

    //    public ConstraintsWriterManager(StringBuilder builder, DbEngine engine) : base(builder, engine)
    //    {
    //    }

    //    protected ConstraintsCollection Constraints
    //    {
    //        get { return _constraints; }
    //    }

    //    public virtual void Write(ConstraintsCollection constraints)
    //    {
    //        _constraints = constraints;
    //        var uniq = GetUniqueKeyWriter();
    //        if (constraints.PrimaryKey != null)
    //        {
    //            Builder.AppendLine();
    //            uniq.Write(constraints.PrimaryKey);
    //            Builder.Append(",");
    //        }

    //        if (constraints.Uniques.Count > 0)
    //        {
    //            foreach (var uc in constraints.Uniques)
    //            {
    //                Builder.AppendLine();
    //                uniq.Write(uc);
    //                Builder.Append(",");
    //            }

    //        }

    //        if (constraints.ForeignKeys.Count > 0)
    //        {
    //            var w = GetForeignKeyWriter();
    //            foreach (var key in constraints.ForeignKeys)
    //            {
    //                Builder.AppendLine();
    //                w.Write(key);
    //                Builder.Append(",");
    //            }
    //        }

    //        if (constraints.Checks.Count > 0)
    //        {
    //            var chkWriter = GetCheckWriter();
    //            foreach (var ch in constraints.Checks)
    //            {
    //                Builder.AppendLine();
    //                chkWriter.Write(ch);
    //                Builder.Append(",");
    //            }
    //        }

    //        var custom = constraints.GetSpecificConstraints(_engine);
    //        if (!custom.IsNullOrEmpty())
    //        {
    //            foreach (var ch in custom)
    //            {
    //                Builder.AppendLine().Append(ch).Append(",");
    //            }
    //        }
    //        Builder.RemoveLastIfEquals(',');

    //    }

    //    public virtual void WriteNewConstraints(ConstraintsCollection constraints)
    //    {
    //        _constraints = constraints;
    //        Builder.Append("alter table ");
    //    }

    //    protected virtual void Write(UniqueKeyConstraint key)
    //    {
            
    //    }
        
    //    protected virtual void Write(ForeignKeyConstraint key)
    //    {
            
    //    }
        
    //    protected virtual void Write(CheckConstraint key)
    //    {
            
    //    }
    //}
}