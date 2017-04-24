namespace Coco_R
{
    /// <summary>
    /// Class in charge of building variables.
    /// </summary>
    internal class VariableBuilder
    {
        /// <summary>
        /// Default value for ints.
        /// </summary>
        private const int EntDefault = 0;

        /// <summary>
        /// Default value for decimals.
        /// </summary>
        private const double DecDefault = 0.0;

        /// <summary>
        /// Default value for booleans.
        /// </summary>
        private const bool BoolDefault = false;

        /// <summary>
        /// Default value for strings.
        /// </summary>
        private const string CadDefault = "";

        /// <summary>
        /// Counter used to name temporary variables.
        /// </summary>
        private int _counter;

        /// <summary>
        /// Creates a new variable object.
        /// </summary>
        /// <param name="type">Type of the variable</param>
        /// <param name="name">Name of the variable. If none is specified, a
        /// temp name is assigned.</param>
        /// <returns>The variable object.</returns>
        public Variable NewVariable(Type type, string name = "")
        {
            dynamic value;
            switch (type)
            {
                case Type.Booleano:
                    value = BoolDefault;
                    break;
                case Type.Cadena:
                    value = CadDefault;
                    break;
                case Type.Decimal:
                    value = DecDefault;
                    break;
                case Type.Entero:
                    value = EntDefault;
                    break;
                default:
                    value = 0;
                    break;
            }

            return new Variable
            {
                Name = name != "" ? name : $"Temp{_counter++}",
                Type = type,
                Value = value
            };
        }
    }
}
