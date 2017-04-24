using System.Drawing;
using System.Windows.Forms;

namespace Compiler
{
    using Coco_R;
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.Write("File address: ");
            args = new[] { Console.ReadLine() };
#endif
            if (args.Length == 0)
            {
                Console.WriteLine("Especifica la dirección del archivo a compilar");
                return;
            }

            var path = args[0];

            if (File.Exists(path))
            {
                var scanner = new Scanner(path);
                var parser = new Parser(scanner);
                var commandList = parser.Parse();

                if (parser.errors.count != 0)
                    Console.WriteLine(parser.errors.count + " errores detectados.");
                else
                {
                    var vm = new VirtualMachine();
                    commandList.ExecuteBy(vm);
                    var frm = new Form { ClientSize = new Size(500, 500) };
                    var pb = new PictureBox { Image = vm.Image, Size = frm.ClientSize };
                    frm.Controls.Add(pb);
                    frm.ShowDialog();
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }

        }
    }
}
