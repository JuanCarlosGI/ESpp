using System.Collections.Generic;

namespace Coco_R
{
    /// <summary>
    /// Represents a command to be executed by the virtual machine.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Part of a visitor design pattern, made for the virtual machine to
        /// recognize and run different types of commands.
        /// </summary>
        /// <param name="vm">The virtual machine.</param>
        public abstract void ExecuteBy(IVirtualMachine vm);
    }

    /// <summary>
    /// Part of a composite design pattern. This is the composite object,
    /// consisting of a list of commands.
    /// </summary>
    public class CommandList : Command
    {
        /// <summary>
        /// Gets or sets the list of commands.
        /// </summary>
        public List<Command> Commands { get; set; } = new List<Command>();

        public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); }
    }

    /// <summary>
    /// Class representing a command where there are two perands and one
    /// separate result.
    /// </summary>
    public abstract class BinaryCommand : Command
    {
        /// <summary>
        /// Gets or sets the first operator.
        /// </summary>
        public DirectValueSymbol Op1 { get; set; }

        /// <summary>
        /// Gets or sets the second operator.
        /// </summary>
        public DirectValueSymbol Op2 { get; set; }

        /// <summary>
        /// Gets or sets the symbol where the value will be stored.
        /// </summary>
        public DirectValueSymbol Result { get; set; }
    }

    /// <summary>
    /// Class representing a sum of two symbols.
    /// </summary>
    public class Sum : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a subtraction fo two symbols.
    /// </summary>
    public class Subtract : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a division of two symbols.
    /// </summary>
    public class Divide : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a multiplication of two symbols.
    /// </summary>
    public class Multiply : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing the modulo of two symbols.
    /// </summary>
    public class Modulo : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing an assign operation.
    /// </summary>
    public class Assign : Command
    {
        /// <summary>
        /// Symbol where a value will be stored.
        /// </summary>
        public DirectValueSymbol Source { get; set; }

        /// <summary>
        /// Symbol from where the value will be obtained.
        /// </summary>
        public DirectValueSymbol Recipient { get; set; }

        public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); }
    }

    /// <summary>
    /// Class representing an equality operation.
    /// </summary>
    public class Equals : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a LESS THAN operation.
    /// </summary>
    public class LessThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a GREATER THAN operation.
    /// </summary>
    public class GreaterThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a DIFFERENT operation.
    /// </summary>
    public class Different : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a GREATER O EQUAL THAN operation.
    /// </summary>
    public class GreaterOrEqualThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a LESS OR EQUAL THAN operation.
    /// </summary>
    public class LessOrEqualThan : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing an AND operation.
    /// </summary>
    public class And : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing an OR operation.
    /// </summary>
    public class Or : BinaryCommand { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing an IF statement command.
    /// </summary>
    public class Conditional : Command
    {
        /// <summary>
        /// Gets or sets the command list that will be executed if the condition
        /// is met.
        /// </summary>
        public CommandList If { get; set; }

        /// <summary>
        /// Gets or sets the command list that will be executed if the condition
        /// is not met.
        /// </summary>
        public CommandList Else { get; set; }

        /// <summary>
        /// Gets or sets the symbol that will be checked to see if the condition
        /// is being met.
        /// </summary>
        public DirectValueSymbol Condition { get; set; }

        public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); }
    }

    /// <summary>
    /// Class representing a command that will be executed over a scope.
    /// </summary>
    public abstract class ScopeOperation : Command
    {
        /// <summary>
        /// Gets or sets the scope that the command will affect.
        /// </summary>
        public Scope Scope { get; set; }
    }

    /// <summary>
    /// Class representing a command that will push new default values into the
    /// scope's symbols.
    /// </summary>
    public class PushDefaults : ScopeOperation { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a command that will pop current values from the
    /// scope's symbols.
    /// </summary>
    public class PopLocals : ScopeOperation { public override void ExecuteBy(IVirtualMachine vm) { vm.Execute(this); } }

    /// <summary>
    /// Class representing a Read command.
    /// </summary>
    public class Read : Command
    {
        /// <summary>
        /// Symbol where the result will be stored.
        /// </summary>
        public DirectValueSymbol Result { get; set; }
        
        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    /// <summary>
    /// Class representing a WHILE command.
    /// </summary>
    public class While : Command
    {
        /// <summary>
        /// The command list that, when executed, will update the condition.
        /// </summary>
        public CommandList Expression { get; set; }

        /// <summary>
        /// The cmommand list that will be executed while the condition is met.
        /// </summary>
        public CommandList WhileBlock { get; set; }

        /// <summary>
        /// The symbol that contains the condition that should be met.
        /// </summary>
        public DirectValueSymbol Result { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    /// <summary>
    /// Class representing a RANDOM command.
    /// </summary>
    public class Random : Command
    {
        /// <summary>
        /// Gets or sets the symbol where the result will be stored.
        /// </summary>
        public DirectValueSymbol Result { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    /// <summary>
    /// Class representing a PRINT command.
    /// </summary>
    public class Print : Command
    {
        /// <summary>
        /// Gets or sets the list of symbols that will be printed.
        /// </summary>
        public List<DirectValueSymbol> Values { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    /// <summary>
    /// Class representing a command that will call and execute a function.
    /// </summary>
    public class CallFunction : Command
    {
        /// <summary>
        /// Gets or sets the function that will be called.
        /// </summary>
        public Function Function { get; set; }

        /// <summary>
        /// Gets or sets the list of parameters that will be assigned.
        /// </summary>
        public List<DirectValueSymbol> Parameters { get; internal set; }

        /// <summary>
        /// Gets or sets the symbol where the result will be stored.
        /// </summary>
        public DirectValueSymbol Result { get; set; }

        /// <summary>
        /// Gets or sets the scope that is being called.
        /// </summary>
        public Scope ScopeCalled { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }

    /// <summary>
    /// Class representing a command that will assign the current index of an 
    /// array.
    /// </summary>
    public class AssignIndex : Command
    {
        /// <summary>
        /// Gets or sets the array object that will be assigned.
        /// </summary>
        public VariableArray Array { get; set; }

        /// <summary>
        /// Gets or sets the symbols where the index value will be obtained.
        /// </summary>
        public List<DirectValueSymbol> Indexes { get; set; }

        public override void ExecuteBy(IVirtualMachine vm)
        {
            vm.Execute(this);
        }
    }
}
