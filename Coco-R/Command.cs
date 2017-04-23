using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public abstract class Command
    {
        public abstract void ExecuteBy(IVirtualMachine vm);
    }

    public class CommandList : Command
    {
        public List<Command> Commands { get; set; } = new List<Command>();

        public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); }
    }

    public abstract class BinaryCommand : Command
    {
        public DirectValueSymbol Op1 { get; set; }
        public DirectValueSymbol Op2 { get; set; }
        public DirectValueSymbol Result { get; set; }
    }
    public class Sum : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Subtract : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Divide : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Multiply : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Modulo : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Assign : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Equals : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class LessThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class GreaterThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Different : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class GreaterOrEqualThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class LessOrEqualThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class And : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }
    public class Or : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    public class Conditional : Command
    {
        public CommandList If { get; set; }
        public CommandList Else { get; set; }
        public DirectValueSymbol Condition { get; set; }

        public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); }
    }

    public abstract class CodeBlockOperation : Command
    {
        public CodeBlock CodeBlock { get; set; }
    }

    public class PushDefaults : CodeBlockOperation { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    public class PopLocals : CodeBlockOperation { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    public class Read : Command
    {
        public DirectValueSymbol Result { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class While : Command
    {
        public CommandList Expression { get; set; }
        public CommandList WhileBlock { get; set; }
        public DirectValueSymbol Result { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class Random : Command
    {
        public DirectValueSymbol Result { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class Print : Command
    {
        public List<DirectValueSymbol> Values { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class CallFunction : Command
    {
        public Function Function { get; set; }
        public List<DirectValueSymbol> Parameters { get; internal set; }
        public DirectValueSymbol Result { get; set; }

        public CodeBlock ScopeCalled { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class AssignParam : Command
    {
        public DirectValueSymbol Parameter { get; set; }

        public DirectValueSymbol Source { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    public class AssignIndex : Command
    {
        public VariableArray Array { get; set; }
        public DirectValueSymbol Index { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }
}
