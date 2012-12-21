using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlFu.DDL
{
    internal class DbEngineOptions : List<DbSpecificOption>
    {
        /// <summary>
        /// Returns null if option doesn't exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbSpecificOption Get(string name, DbEngine engine = DbEngine.None)
        {
            if (engine == DbEngine.None) engine = _currentEngine;
            return Find(opt => opt.Name == name && opt.Engine == engine);
        }

        public DbSpecificOption Get(DbSpecificOption option)
        {
            return Find(o => o.IsSameOption(option));
        }

        /// <summary>
        /// Adds a new option. If the option exists it's replaced
        /// </summary>
        /// <param name="options"></param>
        public new void AddRange(IEnumerable<DbSpecificOption> options)
        {
            options.MustNotBeNull();
            foreach (var o in options)
            {
                var opt = Get(o);
                if (opt != null)
                {
                    Remove(opt);
                }
                Add(o);
            }
        }

        public DbEngineOptions Use(DbEngine engine)
        {
            _currentEngine = engine;
            return this;
        }

        public bool HasAny(params string[] names)
        {
            if (names.IsNullOrEmpty()) return false;
            return this.Any(d => d.Engine == _currentEngine && names.Any(n => n == d.Name));
        }

        //Dictionary<DbEngine, NameValueCollection> _tableOptions = new Dictionary<DbEngine, NameValueCollection>();

        // public void SetOptions(DbEngine db,params string[] options)
        // {

        //     options.MustNotBeEmpty();
        //     NameValueCollection nv;
        //     if (!_tableOptions.ContainsKey(db))
        //     {
        //         nv = new NameValueCollection();
        //         _tableOptions[db] = nv;
        //     }
        //     else
        //     {
        //         nv = _tableOptions[db];
        //     }
        //     foreach(var option in options)
        //     {
        //         var items = option.Split(':');
        //         if (items.Length > 0)
        //         {
        //             string key = items[0].Trim().ToUpperInvariant();
        //             string value = null;
        //             if (items.Length > 1)
        //             {
        //                 value = items[1].Trim().ToUpperInvariant();
        //             }
        //             nv[key] = value;
        //         }
        //     }

        // }

        // Dictionary<DbEngine,string> _removed=new Dictionary<DbEngine, string>();
        private DbEngine _currentEngine;

        ///// <summary>
        ///// Sets the specified option to be removed.
        ///// Used when altering a table element
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="option"></param>
        //internal void MarkOptionForRemoval(DbEngine db,string option)
        //{
        //    option.MustNotBeEmpty();
        //    _removed[db] = option;
        //}

        //internal NameValueCollection GetOptions(DbEngine db)
        //{
        //    NameValueCollection rez = null;
        //    _tableOptions.TryGetValue(db, out rez);
        //    return rez ?? new NameValueCollection();
        //}

        //internal string GetOptionValue(string name,DbEngine db)
        //{
        //    NameValueCollection rez = null;
        //    name=name.ToUpperInvariant();
        //    if (_tableOptions.TryGetValue(db, out rez))
        //    {
        //        var opt=rez[name];

        //        return opt;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Checks if any of the option exists
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="names"></param>
        ///// <returns></returns>
        //internal bool HasOption(DbEngine db, params string[] names)
        //{
        //    NameValueCollection rez = null;

        //    if (_tableOptions.TryGetValue(db, out rez))
        //    {
        //        foreach(var name in names)
        //        {
        //            var name1 = name.ToUpperInvariant();
        //            if (rez.AllKeys.Any(k => k == name1))
        //            {
        //                return true;
        //            }
        //        }

        //    }
        //    return false;
        //}
    }
}