using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public class VirtualMachine : IVirtualMachine
    {
        private readonly System.Random _rand = new System.Random();

        public void Execute(Subtract cmd)
        {
            cmd.Result.Value = cmd.Op1.Value - cmd.Op2.Value;
        }
        public void Execute(Multiply cmd)
        {
            cmd.Result.Value = cmd.Op1.Value * cmd.Op2.Value;
        }
        public void Execute(Assign cmd)
        {
            switch (cmd.Result.Type)
            {
                case Type.Entero:
                    cmd.Result.Value = (int)cmd.Op2.Value;
                    break;
                case Type.Decimal:
                    cmd.Result.Value = (double)cmd.Op2.Value;
                    break;
                default:
                    cmd.Result.Value = cmd.Op2.Value;
                    break;
            }
        }
        public void Execute(LessThan cmd)
        {
            cmd.Result.Value = cmd.Op1.Value < cmd.Op2.Value;
        }
        public void Execute(Different cmd)
        {
            cmd.Result.Value = cmd.Op1.Value != cmd.Op2.Value;
        }
        public void Execute(LessOrEqualThan cmd)
        {
            cmd.Result.Value = cmd.Op1.Value <= cmd.Op2.Value;
        }
        public void Execute(Or cmd)
        {
            cmd.Result.Value = cmd.Op1.Value || cmd.Op2.Value;
        }
        public void Execute(PushDefaults cmd)
        {
            cmd.CodeBlock.pushDefaultValues();
        }
        public void Execute(Read cmd)
        {
            var line = Console.ReadLine();

            cmd.Result.Value = line;
        }
        public void Execute(Print cmd)
        {
            var final = "";
            foreach(var symbol in cmd.Values)
            {
                final += $"{symbol.Value} " ;
            }
            final = final.Substring(0, final.Length - 1);

            Console.WriteLine(final);
        }
        public void Execute(AssignParam cmd)
        {
            cmd.Parameter.Value = cmd.Source.Value;
        }
        public void Execute(AssignIndex cmd)
        {
            cmd.Array.Index = cmd.Index;
        }
        public void Execute(CallFunction cmd)
        {
            var commands = cmd.Function.CommandList;

            if (commands == null) return;

            Stack<AssignParam> stack = new Stack<AssignParam>();
            while (commands.Commands[1] is AssignParam)
            {
                stack.Push(commands.Commands[1] as AssignParam);
                commands.Commands.RemoveAt(1);
            }

            for (int para = cmd.Parameters.Count - 1; para >= 0; para--)
            {
                var parameter = cmd.Function.Parameters[para];
                var newCmd = new AssignParam { Parameter = parameter, Source = cmd.Parameters[para] };
                commands.Commands.Insert(1, newCmd);
            }

            Execute(commands);

            for (int para = cmd.Parameters.Count - 1; para >= 0; para--)
            {
                commands.Commands.RemoveAt(1);
            }

            while (stack.Count != 0)
            {
                commands.Commands.Insert(1, stack.Pop());
            }

            if (cmd.Function.Type != Type.Rutina)
            {
                var aux = cmd.Function.Returns.Value;
                cmd.Function.Returns.Unroll();
                cmd.Result.Value = aux;
            }
        }
        public void Execute(Random cmd)
        {
            var result = _rand.Next(int.MaxValue) / (double)int.MaxValue;

            cmd.Result.Value = result;
        }
        public void Execute(While cmd)
        {
            Execute(cmd.Expression);
            while (cmd.Result.Value)
            {
                Execute(cmd.WhileBlock);
                Execute(cmd.Expression);
            }
        }
        public void Execute(PopLocals cmd)
        {
            cmd.CodeBlock.popLocalValues();
        }
        public void Execute(Conditional cmd)
        {
            if (cmd.Condition.Value)
                Execute(cmd.If);
            else if (cmd.Else != null)
                Execute(cmd.Else);
        }
        public void Execute(And cmd)
        {
            cmd.Result.Value = cmd.Op1.Value && cmd.Op2.Value;
        }
        public void Execute(GreaterOrEqualThan cmd)
        {
            cmd.Result.Value = cmd.Op1.Value >= cmd.Op2.Value;
        }
        public void Execute(GreaterThan cmd)
        {
            cmd.Result.Value = cmd.Op1.Value > cmd.Op2.Value;
        }
        public void Execute(Equals cmd)
        {
            cmd.Result.Value = cmd.Op1.Value == cmd.Op2.Value;
        }
        public void Execute(Modulo cmd)
        {
            cmd.Result.Value = cmd.Op1.Value % cmd.Op2.Value;
        }
        public void Execute(Divide cmd)
        {
            cmd.Result.Value = cmd.Op1.Value / cmd.Op2.Value;
        }
        public void Execute(Sum cmd)
        {
            cmd.Result.Value = cmd.Op1.Value + cmd.Op2.Value;
        }
        public void Execute(CommandList commands)
        {
            for (int i = 0; i < commands.Commands.Count; i++)
            {
                var command = commands.Commands[i];
                command.ExecuteBy(this);
            }
        }
    }
}
