using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public abstract class Symbol
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public abstract class DirectValueSymbol : Symbol
    {
        public virtual dynamic Value { get; set; }
    }

    class Constant : DirectValueSymbol { }

    public class Variable : DirectValueSymbol
    {
        private Stack<dynamic> Values = new Stack<dynamic>();

        public override dynamic Value {
            get
            {
                return Values.Peek();
            }
            set
            {
                if (Values.Count > 0) Values.Pop();
                Values.Push(value);
            }
        }

        public void SaveAndClear(dynamic value)
        {
            Values.Push(value);
        }

        public void Unroll()
        {
            Values.Pop();
        }
    }

    class VariableArray : Symbol
    {
        public int Length { get; set; }

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
                    Value = EsConstantBuilder.DefaultValue(Type)
                };
            }
        }
    }

    class Function : Symbol
    {
        public List<Variable> Parameters { get; set; }
    }
}
