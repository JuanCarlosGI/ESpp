﻿namespace Compiler
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
                parser.Parse();
                Console.WriteLine(parser.errors.count + " errors detected.");
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }
        }
    }
}
