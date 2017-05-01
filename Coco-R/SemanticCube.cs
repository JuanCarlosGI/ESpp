namespace Coco_R
{
    public partial class Parser
    {
        /// <summary>
        /// Semantic cube to determine result type for all operations.
        /// </summary>
        private readonly Type[,,] _cube =
        {
            {
                // Entero vs entero
                {
                    Type.Entero, Type.Entero, Type.Entero, Type.Entero, Type.Entero,
                    Type.Entero, Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano,
                    Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano
                },

                // Entero vs decimal
                {
                    Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal,
                    Type.Entero, Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano,
                    Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano
                },

                // Entero vs booleano
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Entero vs cadena
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                }
            },
            {
                // Decimal vs entero
                {
                    Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal,
                    Type.Decimal, Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano,
                    Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano
                },

                // Decimal vs decimal
                {
                    Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal, Type.Decimal,
                    Type.Decimal, Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano,
                    Type.Booleano, Type.Booleano, Type.Booleano, Type.Booleano
                },

                // Deicmal vs booleano
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Decimal vs cadena
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                }
            },
            {
                // Booleano vs entero
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Booleano vs decimal
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Booleano vs booleano
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Booleano,
                    Type.Booleano, Type.Error,Type.Error,Type.Booleano,Type.Error,Type.Error,
                    Type.Booleano,Type.Booleano},

                // Booleano vs cadena
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                }
            },
            {
                // Cadena vs entero
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Cadena vs decimal
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Cadena vs booleano
                {
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,Type.Error,
                    Type.Error,Type.Error
                },

                // Cadena vs cadena
                {
                    Type.Cadena,Type.Error,Type.Error,Type.Error,Type.Error,Type.Cadena,
                    Type.Booleano,Type.Error,Type.Error,Type.Booleano,Type.Error,Type.Error,
                    Type.Error,Type.Error}
            }
        };
    }
}
