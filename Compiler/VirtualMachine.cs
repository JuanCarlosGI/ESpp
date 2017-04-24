using System;
using System.Collections.Generic;
using Coco_R;
using Random = Coco_R.Random;
using Type = Coco_R.Type;

namespace Compiler
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
            switch (cmd.Recipient.Type)
            {
                case Type.Entero:
                    cmd.Recipient.Value = (int)cmd.Source.Value;
                    break;
                case Type.Decimal:
                    cmd.Recipient.Value = (double)cmd.Source.Value;
                    break;
                case Type.Booleano:
                    cmd.Recipient.Value = (bool)cmd.Source.Value;
                    break;
                case Type.Cadena:
                    cmd.Recipient.Value = (string)cmd.Source.Value;
                    break;
                default:
                    cmd.Recipient.Value = cmd.Source.Value;
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
            cmd.Scope.PushDefaultValues();
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

        public void Execute(AssignIndex cmd)
        {
            cmd.Array.Index = cmd.Index;
        }
        public void Execute(CallFunction cmd)
        {
            var commands = cmd.Function.CommandList;

            if (commands == null) return;

            Stack<Assign> stack = new Stack<Assign>();
            while (commands.Commands[1] is Assign)
            {
                stack.Push((Assign)commands.Commands[1]);
                commands.Commands.RemoveAt(1);
            }

            for (int para = cmd.Parameters.Count - 1; para >= 0; para--)
            {
                var parameter = cmd.Function.Parameters[para];
                var newCmd = new Assign { Recipient = parameter, Source = cmd.Parameters[para] };
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
            cmd.Scope.PopLocalValues();
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
