using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Coco_R
{
    /// <summary>
    /// Class representing a scope in the code.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="parent">The parent scope.</param>
        /// <param name="name">The name of the scope.</param>
        public Scope(Scope parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the parent of the scope.
        /// </summary>
        public Scope Parent { get; set; }

        /// <summary>
        /// Gets or sets the list of children of the scope.
        /// </summary>
        public List<Scope> Children { get; set; } = new List<Scope>();

        /// <summary>
        /// Gets or sets the name of the scope.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Hash containing all local symbols.
        /// </summary>
        private readonly Hashtable _hash = new Hashtable();

        /// <summary>
        /// Gets all the symbols in the hash.
        /// </summary>
        public IEnumerable<Symbol> Symbols => _hash.Values.Cast<Symbol>();

        /// <summary>
        /// Seaches for a symbol in all available scopes prioritizing the 
        /// current scope and working its way up to the global scope.
        /// </summary>
        /// <param name="name">Name of the symbol to be found.</param>
        /// <returns>The symbol, or null if it is not found.</returns>
        public Symbol Search(string name)
        {
            var symbol = _hash[name] as Symbol;
            if (symbol == null && Parent != null)
            {
                symbol = Parent.Search(name);
            }

            return symbol;
        }

        /// <summary>
        /// Searches for the outermost scope of a function, prioritizing the
        /// current scope and working its way up to the global scope.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The function, or null if not found.</returns>
        public Scope SearchForFunctionScope(string name)
        {
            var block = Children.Find(c => c.Name == name);
            return block ?? Parent?.SearchForFunctionScope(name);
        }

        /// <summary>
        /// Adds a symbol to the symbol hash.
        /// </summary>
        /// <param name="symbol">Symbol to add.</param>
        public void Add(Symbol symbol)
        {
            if (_hash[symbol.Name] != null)
                throw new InvalidOperationException($"Variable {symbol.Name} is already defined in this scope.");

            _hash[symbol.Name] = symbol;
        }

        /// <summary>
        /// Determines whether a symbol with a given name exists in the current
        /// scope. Note that it does not search in its parent scope.
        /// </summary>
        /// <param name="name">Name of the symbol.</param>
        /// <returns></returns>
        public bool ExistsInScope(string name)
        {
            return _hash[name] != null;
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandList"/> that this scope is
        /// composed of.
        /// </summary>
        public CommandList CommandList { get; set; } = new CommandList();

        /// <summary>
        /// Gets or sets the variable in which the return value, if it has one,
        /// will be stored during execution.
        /// </summary>
        public DirectValueSymbol Returns { get; set; }

        /// <summary>
        /// Pushes the default values into every symbol in its hash.
        /// </summary>
        public void PushDefaultValues()
        {
            foreach (var obj in _hash.Values)
            {
                if (!(obj is DirectValueSymbol)) continue;

                var variable = (DirectValueSymbol)obj;
                variable.SaveAndClear(ConstantBuilder.DefaultValue(variable.Type));
            }
        }

        /// <summary>
        /// Pops the current value from all its symbols, except for the one with
        /// the return value.
        /// </summary>
        public void PopLocalValues()
        {
            foreach (var obj in _hash.Values)
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
