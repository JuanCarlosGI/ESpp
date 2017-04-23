namespace Coco_R
{
    public interface IVirtualMachine
    {
        void Execute(Subtract cmd);
        void Execute(Multiply cmd);
        void Execute(Assign cmd);
        void Execute(LessThan cmd);
        void Execute(Different cmd);
        void Execute(LessOrEqualThan cmd);
        void Execute(Or cmd);
        void Execute(PushDefaults cmd);
        void Execute(Read cmd);
        void Execute(Print cmd);
        void Execute(AssignParam cmd);
        void Execute(AssignIndex cmd);
        void Execute(CallFunction cmd);
        void Execute(Random cmd);
        void Execute(While cmd);
        void Execute(PopLocals cmd);
        void Execute(Conditional cmd);
        void Execute(And cmd);
        void Execute(GreaterOrEqualThan cmd);
        void Execute(GreaterThan cmd);
        void Execute(Equals cmd);
        void Execute(Modulo cmd);
        void Execute(Divide cmd);
        void Execute(Sum cmd);
        void Execute(CommandList commands);
    }
}
