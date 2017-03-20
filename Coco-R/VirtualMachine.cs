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
    }

    public partial class Parser : VirtualMachine
    {
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
            foreach(var command in commands.Commands)
            {
                command.ExecuteBy(this);
            }
        }
    }
}
