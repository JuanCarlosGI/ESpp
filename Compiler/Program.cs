﻿using System.Drawing;
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
                CommandList commandList = null;
                try
                {
                    commandList = parser.Parse();
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("Exception during parsing process:");
                    Console.WriteLine(e);
#endif
                }
                if (parser.errors.count != 0 || commandList == null)
                    Console.WriteLine(parser.errors.count + " errores detectados.");
                else
                {
                    try
                    {
                        var vm = new VirtualMachine();
                        commandList.ExecuteBy(vm);
                        var frm = new Form
                        {
                            ClientSize = new Size(1000, 1000),
                            Text = @"Resultado",
                            Icon = Properties.Resources.Logo,
                            FormBorderStyle = FormBorderStyle.FixedSingle,
                            MaximizeBox = false
                        };
                        var pb = new PictureBox
                        {
                            Image = vm.Image,
                            Size = frm.ClientSize,
                            SizeMode = PictureBoxSizeMode.StretchImage
                        };
                        frm.Controls.Add(pb);
                        frm.ShowDialog();
                    }
                    catch (EsppException e)
                    {
                        Console.WriteLine("ERROR:");
                        Console.WriteLine($"\t{e.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }
        }
    }
}
