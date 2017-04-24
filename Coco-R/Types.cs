namespace Coco_R
{
    /// <summary>
    /// Types to be used by the compiler.
    /// </summary>
    public enum Type
    {
        Entero,
        Decimal,
        Booleano,
        Cadena,
        Rutina,
        Error
    }

    /// <summary>
    /// Operators to be used by the compiler.
    /// </summary>
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
