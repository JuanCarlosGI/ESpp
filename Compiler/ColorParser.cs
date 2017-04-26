using System.Drawing;

namespace Compiler
{
    class ColorParser
    {
        public Color Parse(string col)
        {
            switch (col)
            {
                case "azul":
                    return Color.Blue;
                case "amarillo":
                    return Color.Yellow;
                case "rojo":
                    return Color.Red;
                case "morado":
                    return Color.Purple;
                case "verde":
                    return Color.Green;
                case "naranja":
                    return Color.Orange;
                case "rosa":
                    return Color.Pink;
                case "cafe":
                    return Color.Brown;
                default:
                    return Color.Black;
            }
        }
    }
}
