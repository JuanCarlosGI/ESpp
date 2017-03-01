namespace Compiler
{
    using Coco_R;
    using System;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Dirección del archivo: ");
            var path = Console.ReadLine();

            if (File.Exists(path))
            {
                var scanner = new Scanner(path);
                var parser = new Parser(scanner);
                parser.Parse();
                Console.WriteLine(parser.errors.count + " errors detected");
            }
            else
            {
                Console.WriteLine("Dirección inválida");
            }
        }
    }
}
