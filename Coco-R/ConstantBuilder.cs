using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    internal class ConstantBuilder
    {
        private const int IntDefault = 0;
        private const double DecDefault = 0.0;
        private const bool BoolDefault = false;
        private const string StrDefault = "";

        public Constant IntConstant(string value)
        {
            return new Constant
            {
                Value = int.Parse(value),
                Type = Type.Entero,
                Name = value
            };
        }

        public Constant DecConstant(string value)
        {
            return new Constant
            {
                Value = double.Parse(value),
                Type = Type.Decimal,
                Name = value
            };
        }

        public Constant StrConstant(string value)
        {
            return new Constant
            {
                Value = value.Length >= 2 ? value.Substring(1,value.Length - 2) : "",
                Type = Type.Cadena,
                Name = value
            };
        }

        public Constant BoolConstant(string value)
        {
            return new Constant
            {
                Value = value == "verdadero",
                Type = Type.Booleano,
                Name = value
            };
        }

        public static object DefaultValue(Type type)
        {
            if (type == Type.Entero) return IntDefault;
            if (type == Type.Decimal) return DecDefault;
            if (type == Type.Cadena) return StrDefault;
            return BoolDefault;
        }
    }
}
