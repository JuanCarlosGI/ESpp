namespace Coco_R
{
    /// <summary>
    /// Class in charge of building constant objects.
    /// </summary>
    internal class ConstantBuilder
    {
        /// <summary>
        /// The default value for ints.
        /// </summary>
        private const int IntDefault = 0;

        /// <summary>
        /// The default value for decimals.
        /// </summary>
        private const double DecDefault = 0.0;

        /// <summary>
        /// The default value for booleans.
        /// </summary>
        private const bool BoolDefault = false;

        /// <summary>
        /// The default value for strings.
        /// </summary>
        private const string StrDefault = "";

        /// <summary>
        /// Builds a new int constant with the given value.
        /// </summary>
        /// <param name="value">The value that the constant will contain.</param>
        /// <returns>The built constant.</returns>
        public Constant IntConstant(string value)
        {
            return new Constant
            {
                Value = int.Parse(value),
                Type = Type.Entero,
                Name = $"@{value}"
            };
        }

        /// <summary>
        /// Build a new decimal constant.
        /// </summary>
        /// <param name="value">The value that the constant will have.</param>
        /// <returns>The built constant.</returns>
        public Constant DecConstant(string value)
        {
            return new Constant
            {
                Value = double.Parse(value),
                Type = Type.Decimal,
                Name = $"@{value}"
            };
        }

        /// <summary>
        /// Builds a new string constant.
        /// </summary>
        /// <param name="value">The value that the constant will have.</param>
        /// <returns>The built constant.</returns>
        public Constant StrConstant(string value)
        {
            return new Constant
            {
                Value = value.Length >= 2 ? value.Substring(1,value.Length - 2) : "",
                Type = Type.Cadena,
                Name = $"@{value}"
            };
        }

        /// <summary>
        /// Builds a new boolean constant.
        /// </summary>
        /// <param name="value">The value that the constant will have.</param>
        /// <returns>The built constant.</returns>
        public Constant BoolConstant(string value)
        {
            return new Constant
            {
                Value = value == "verdadero",
                Type = Type.Booleano,
                Name = $"@{value}"
            };
        }

        /// <summary>
        /// Gets the default value for a symbol of a given type.
        /// </summary>
        /// <param name="type">The type of the symbol</param>
        /// <returns>The default value</returns>
        public static dynamic DefaultValue(Type type)
        {
            switch (type)
            {
                case Type.Entero:
                    return IntDefault;
                case Type.Decimal:
                    return DecDefault;
                case Type.Cadena:
                    return StrDefault;
                case Type.Booleano:
                    return BoolDefault;
                default:
                    return null;
            }
        }
    }
}
