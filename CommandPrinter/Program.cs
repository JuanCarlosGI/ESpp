﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coco_R;

namespace CommandPrinter
{
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
                    var vm = new CommandPrinter($"results.txt");
                    commandList.ExecuteBy(vm);

                    while (vm.PrintNext()) { }
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }
        }
    }
}
