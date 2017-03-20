using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public abstract class Command
    {
        public abstract void ExecuteBy(VirtualMachine vm);
    }

    public class CommandList : Command
    {
        public List<Command> Commands { get; set; } = new List<Command>();

        public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); }
    }

    public abstract class BinaryCommand : Command
    {
        public DirectValueSymbol Op1 { get; set; }
        public DirectValueSymbol Op2 { get; set; }
        public DirectValueSymbol Result { get; set; }
    }
    public class Sum : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Subtract : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Divide : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Multiply : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Modulo : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Assign : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Equals : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class LessThan : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class GreaterThan : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Different : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class GreaterOrEqualThan : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class LessOrEqualThan : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class And : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
    public class Or : BinaryCommand { public override void ExecuteBy(VirtualMachine vm) { vm.Execute(this); } }
}
