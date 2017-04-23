using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public interface VirtualMachine
    {
        void Execute(CommandList commands);

        void Execute(Sum cmd);
        void Execute(Subtract cmd);
        void Execute(Divide cmd);
        void Execute(Multiply cmd);
        void Execute(Modulo cmd);
        void Execute(Assign cmd);
        void Execute(Equals cmd);
        void Execute(LessThan cmd);
        void Execute(GreaterThan cmd);
        void Execute(Different cmd);
        void Execute(GreaterOrEqualThan cmd);
        void Execute(LessOrEqualThan cmd);
        void Execute(And cmd);
        void Execute(Or cmd);
        void Execute(Conditional conditional);
        void Execute(PushDefaults pushDefaults);
        void Execute(PopLocals popLocals);
        void Execute(Read lectura);
        void Execute(While @while);
        void Execute(Random random);
        void Execute(Print print);
        void Execute(CallFunction callFunction);
        void Execute(AssignParam assignParam);
        void Execute(AssignIndex assignIndex);
    }

    public partial class Parser : VirtualMachine
    {
        private System.Random _rand = new System.Random();

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
        public void Execute(PushDefaults pushDefaults)
        {
            pushDefaults.CodeBlock.pushDefaultValues();
        }
        public void Execute(Read lectura)
        {
            var line = Console.ReadLine();

            lectura.Result.Value = line;
        }
        public void Execute(Print print)
        {
            var final = "";
            foreach(var symbol in print.Values)
            {
                final += $"{symbol.Value} " ;
            }
            final = final.Substring(0, final.Length - 1);

            Console.WriteLine(final);
        }
        public void Execute(AssignParam assignParam)
        {
            assignParam.Parameter.Value = assignParam.Source.Value;
        }
        public void Execute(AssignIndex assignIndex)
        {
            assignIndex.Array.Index = assignIndex.Index;
        }
        public void Execute(CallFunction callFunction)
        {
            var commands = callFunction.Function.FindCommands(callFunction.ScopeCalled);

            if (commands == null) return;

            Stack<AssignParam> stack = new Stack<AssignParam>();
            while (commands.Commands[1] is AssignParam)
            {
                stack.Push(commands.Commands[1] as AssignParam);
                commands.Commands.RemoveAt(1);
            }

            for (int para = callFunction.Parameters.Count - 1; para >= 0; para--)
            {
                var parameter = callFunction.Function.Parameters[para];
                var cmd = new AssignParam { Parameter = parameter, Source = callFunction.Parameters[para] };
                commands.Commands.Insert(1, cmd);
            }

            Execute(commands);

            for (int para = callFunction.Parameters.Count - 1; para >= 0; para--)
            {
                commands.Commands.RemoveAt(1);
            }

            while (stack.Count != 0)
            {
                commands.Commands.Insert(1, stack.Pop());
            }

            if (callFunction.Function.Type != Type.Rutina)
            {
                var aux = callFunction.Function.Returns.Value;
                callFunction.Function.Returns.Unroll();
                callFunction.Result.Value = aux;
            }
        }
        public void Execute(Random random)
        {
            var result = _rand.Next(int.MaxValue) / (double)int.MaxValue;

            random.Result.Value = result;
        }
        public void Execute(While @while)
        {
            Execute(@while.Expression);
            while (@while.Result.Value)
            {
                Execute(@while.WhileBlock);
                Execute(@while.Expression);
            }
        }
        public void Execute(PopLocals popLocals)
        {
            popLocals.CodeBlock.popLocalValues();
        }
        public void Execute(Conditional conditional)
        {
            if (conditional.Condition.Value)
                Execute(conditional.If);
            else if (conditional.Else != null)
                Execute(conditional.Else);
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
