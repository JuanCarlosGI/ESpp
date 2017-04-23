using System;
using System.Collections.Generic;
using System.Collections;

namespace Coco_R
{
    public class CodeBlock
    {
        public CodeBlock(CodeBlock parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public CodeBlock Parent { get; set; }

        public List<CodeBlock> Children { get; set; } = new List<CodeBlock>();

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

        public CodeBlock SearchForFunctionScope(string name)
        {
            if (Parent != null)
                return Parent.SearchForFunctionScope(name);

            return Children.Find(c => c.Name == name);
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

        public CommandList CommandList = new CommandList();

        public DirectValueSymbol Returns { get; set; }

        public void pushDefaultValues()
        {
            foreach (var obj in hash.Values)
            {
                if (obj is DirectValueSymbol)
                {
                    var variable = obj as DirectValueSymbol;
                    variable.SaveAndClear(ConstantBuilder.DefaultValue(variable.Type));
                }
            }
        }

        public void popLocalValues()
        {
            foreach (var obj in hash.Values)
            {
                if (obj is DirectValueSymbol && obj != Returns)
                {
                    var variable = obj as DirectValueSymbol;
                    variable.Unroll();
                }
            }
        }
    }
}
