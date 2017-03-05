using System;
using System.Collections.Generic;
using System.Collections;

namespace Coco_R
{
    public enum Type
    {
        Entero,
        Decimal,
        Booleano,
        Cadena,
        Rutina
    }

    public class SymbolTable
    {
        public SymbolTable(SymbolTable parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public SymbolTable Parent { get; set; }

        public List<SymbolTable> Children { get; set; } = new List<SymbolTable>();

        public string Name { get; set; }

        private Hashtable hash = new Hashtable();

        public Symbol Search(string name)
        {
            var symbol = hash[name] as Symbol;
            if (symbol == null && Parent != null)
            {
                symbol = Parent.Search(name) as Symbol;
            }

            return symbol;
        }

        public void Add(Symbol symbol)
        {
            if (hash[symbol.Name] != null)
                throw new InvalidOperationException($"Variable {symbol.Name} is already defined in this scope.");

            hash[symbol.Name] = symbol;
        }

        public bool ExistsInScope(string name)
        {
            return hash[name] != null;
        }
    }

    public abstract class Symbol
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class Variable : Symbol
    {
        public bool IsArray { get; set; }
        public int ArrayLength { get; set; }
    }

    class Function : Symbol
    {
        public List<Variable> Parameters { get; set; }
    }
}
