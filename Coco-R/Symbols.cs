using System.Collections.Generic;

namespace Coco_R
{
    public abstract class Symbol
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public abstract class DirectValueSymbol : Symbol
    {
        private readonly Stack<dynamic> _values = new Stack<dynamic>();

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

        public virtual void SaveAndClear(dynamic value)
        {
            _values.Push(value);
        }

        public virtual void Unroll()
        {
            _values.Pop();
        }
    }

    class Constant : DirectValueSymbol { }

    public class Variable : DirectValueSymbol
    {
       
    }

    public class VariableArray : Variable
    {
        public int Length { get; set; }

        public DirectValueSymbol Index { get; set; }

        public override dynamic Value
        {
            get
            {
                return Variables[Index.Value].Value;
            }

            set
            {
                Variables[Index.Value].Value = value;
            }
        }

        public override void SaveAndClear(dynamic value)
        {
            foreach (var variable in Variables)
            {
                variable.SaveAndClear(value);
            }
        }

        public override void Unroll()
        {
            foreach (var variable in Variables)
            {
                variable.Unroll();
            }
        }

        public Variable[] Variables { get; set; }

        public VariableArray(int length)
        {
            Length = length;
            Variables = new Variable[length];

            for (int i = 0; i < length; i++)
            {
                Variables[i] = new Variable
                {
                    Type = Type,
                    Value = ConstantBuilder.DefaultValue(Type)
                };
            }
        }
    }

    public class Function : Symbol
    {
        public List<Variable> Parameters { get; set; }
        public DirectValueSymbol Returns { get; set; }
        public CommandList CommandList { get; set; }
    }
}
