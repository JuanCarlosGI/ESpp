using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    interface ConstantBuilder
    {
        Constant IntConstant(string value);
        Constant DecConstant(string value);
        Constant StrConstant(string value);
        Constant BoolConstant(string value);
    }

    class EsConstantBuilder : ConstantBuilder
    {
        private static readonly int _intDefault = 0;
        private static readonly double _decDefault = 0.0;
        private static readonly bool _boolDefault = false;
        private static readonly string _strDefault = "";

        public Constant IntConstant(string value)
        {
            return new Constant
            {
                Value = int.Parse(value),
                Type = Type.Entero
            };
        }

        public Constant DecConstant(string value)
        {
            return new Constant
            {
                Value = double.Parse(value),
                Type = Type.Decimal
            };
        }

        public Constant StrConstant(string value)
        {
            return new Constant
            {
                Value = value.Length >= 2 ? value.Substring(1,value.Length - 2) : "",
                Type = Type.Cadena
            };
        }

        public Constant BoolConstant(string value)
        {
            return new Constant
            {
                Value = value == "verdadero" ? true : false,
                Type = Type.Booleano
            };
        }

        public static object DefaultValue(Type type)
        {
            if (type == Type.Entero) return _intDefault;
            if (type == Type.Decimal) return _decDefault;
            if (type == Type.Cadena) return _strDefault;
            return _boolDefault;
        }
    }
}
