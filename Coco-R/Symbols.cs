using System;
using System.Collections.Generic;
using System.Linq;

namespace Coco_R
{
    /// <summary>
    /// Class representing a symbol that can be used by the compiler
    /// </summary>
    public abstract class Symbol
    {
        /// <summary>
        /// Gets or sets the name of the symbol.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the symbol.
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// Class representing a symbol which has a valua that can be called
    /// directly.
    /// </summary>
    public abstract class DirectValueSymbol : Symbol
    {
        /// <summary>
        /// Stack storing all values, according to the execution stack.
        /// </summary>
        private readonly Stack<dynamic> _values = new Stack<dynamic>();

        /// <summary>
        /// Gets or sets the current value of the symbol.
        /// </summary>
        public virtual dynamic Value
        {
            get
            {
                return _values.Peek();
            }
            set
            {
                if (_values.Count > 0) _values.Pop();
                _values.Push(value);
            }
        }

        /// <summary>
        /// Saves the current value in the stack and pushes the new value.
        /// </summary>
        /// <param name="value">The new value to be stored.</param>
        public virtual void SaveAndClear(dynamic value)
        {
            _values.Push(value);
        }

        /// <summary>
        /// Pops the current value from the stack of values.
        /// </summary>
        public virtual void Unroll()
        {
            _values.Pop();
        }
    }

    /// <summary>
    /// Class representing a constant.
    /// </summary>
    public class Constant : DirectValueSymbol { }

    /// <summary>
    /// Class representing a variable.
    /// </summary>
    public class Variable : DirectValueSymbol { }

    /// <summary>
    /// Class representing an array of variables.
    /// </summary>
    public class VariableArray : Variable
    {
        /// <summary>
        /// Gets or sets the lengths of the array.
        /// </summary>
        public List<int> Lengths { get; set; }

        /// <summary>
        /// The symbol that will provide the index when accessing the value of
        /// the array.
        /// </summary>
        public List<DirectValueSymbol> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the value of the variable according to the index.
        /// </summary>
        public override dynamic Value
        {
            get
            {
                return Variables[GetActualIndex()].Value;
            }

            set
            {
                Variables[GetActualIndex()].Value = value;
            }
        }

        private int GetActualIndex()
        {
            var accum = 0;
            for (var i = 0; i < Indexes.Count; i++)
            {
                var value = Indexes[i].Value;
                if (value < 0 || value >= Lengths[i])
                {
                    throw new Exception("Out of bounds.");
                }

                accum = accum * Lengths[i] + value;
            }

            return accum;
        }

        /// <summary>
        /// Saves each indexed variable and pushes new ones with the desired
        /// value.
        /// </summary>
        /// <param name="value">The new value to be saved.</param>
        public override void SaveAndClear(dynamic value)
        {
            foreach (var variable in Variables)
            {
                variable.SaveAndClear(value);
            }
        }

        /// <summary>
        /// Pops the current values of all its variables from their respective
        /// stacks.
        /// </summary>
        public override void Unroll()
        {
            foreach (var variable in Variables)
            {
                variable.Unroll();
            }
        }

        /// <summary>
        /// Array of variables contained by the array.
        /// </summary>
        public Variable[] Variables { get; set; }

        /// <summary>
        /// Inicializes a new instance of the <see cref="VariableArray"/> class.
        /// </summary>
        /// <param name="lengths">The lengths of the array.</param>
        public VariableArray(List<int> lengths)
        {
            Lengths = lengths;
            var totalLength = lengths.Aggregate((accum, next) => accum * next);
            Variables = new Variable[totalLength];

            for (var i = 0; i < totalLength; i++)
            {
                Variables[i] = new Variable
                {
                    Type = Type,
                    Value = ConstantBuilder.DefaultValue(Type)
                };
            }
        }
    }

    /// <summary>
    /// Represents a function.
    /// </summary>
    public class Function : Symbol
    {
        /// <summary>
        /// Gets or sets a list of variables with the function's parameters.
        /// </summary>
        public List<Variable> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the symbol where the return value will be stored.
        /// </summary>
        public DirectValueSymbol Returns { get; set; }

        /// <summary>
        /// Gets or sets the list of commands that the function runs.
        /// </summary>
        public CommandList CommandList { get; set; }
    }
}
