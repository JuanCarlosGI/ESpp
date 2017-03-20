using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public enum Type
    {
        Entero,
        Decimal,
        Booleano,
        Cadena,
        Rutina,
        Error
    }

    public enum Operator
    {
        Sum,
        Minus,
        Divide,
        Multiply,
        Modulo,
        Asignation,
        Equality,
        LessThan,
        GreaterThan,
        Different,
        GreaterEqual,
        LessEqual,
        And,
        Or,
        FakeLimit
    }
}
