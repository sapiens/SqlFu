using System;
using System.Data;
using System.Reflection.Emit;

namespace SqlFu
{
    public interface IMapComplexType
    {
        bool IsComplex(string value);

        /// <summary>
        /// Instance of poco is already pushed on the stack
        /// </summary>
        /// <param name="il"></param>
        /// <param name="poco"> </param>
        /// <param name="i"> </param>
        /// <returns></returns>
        bool EmitMapping(ILGenerator il, Type poco,IDataReader rd, int i);

        void MapType<T>(T poco,IDataReader rd,int idx);
        void DeclareILVariables(ILGenerator il);
    }
}