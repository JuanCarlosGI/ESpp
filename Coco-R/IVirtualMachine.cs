namespace Coco_R
{
    /// <summary>
    /// Interface for an object that serves as a virtual machine.
    /// </summary>
    public interface IVirtualMachine
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Subtract cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Multiply cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Assign cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(LessThan cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Different cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(LessOrEqualThan cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Or cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(PushDefaults cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Read cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Print cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(AssignIndex cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(CallFunction cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Random cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(While cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(PopLocals cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Conditional cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(And cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(GreaterOrEqualThan cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(GreaterThan cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Equals cmd);
        void Execute(AssignValue assignValue);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Modulo cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Divide cmd);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        void Execute(Sum cmd);

        /// <summary>
        /// Executes a command list.
        /// </summary>
        /// <param name="commands">The commands.</param>
        void Execute(CommandList commands);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd">The commands</param>
        void Execute(Line cmd);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd">The command</param>
        void Execute(Arc cmd);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd">The command</param>
        void Execute(Rectan cmd);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd">The command</param>
        void Execute(Ellipse cmd);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd"></param>
        void Execute(Triangle cmd);

    }
}
