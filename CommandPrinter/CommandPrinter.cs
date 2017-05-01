using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coco_R;
using Random = Coco_R.Random;

namespace CommandPrinter
{
    class CommandPrinter : IVirtualMachine
    {
        private readonly ICollection<Function> _processedFunctions = new List<Function>();
        private readonly List<Function> _pendingFunctions = new List<Function>();
        private readonly StreamWriter _file;
        private string _indents = "";

        public bool PrintNext()
        {
            if (_pendingFunctions.Count == 0)
            {
                _file.Flush();
                _file.Close();
                return false;
            }

            var function = _pendingFunctions.First();
            _pendingFunctions.Remove(function);
            _file.WriteLine($"\n{_indents}{function.Name}\tParameters:{string.Join(", ", function.Parameters.Select(v => v.Name))};Returns:{(function.Returns != null ? function.Returns.Name : "")}");
            function.CommandList.ExecuteBy(this);
            return true;
        }

        public CommandPrinter(string fileName)
        {
            _file = File.CreateText(fileName);
        }

        public void Execute(Subtract cmd)
        {
            _file.WriteLine($"{_indents}Subtract\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Multiply cmd)
        {
            _file.WriteLine($"{_indents}Multiply\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Assign cmd)
        {
            _file.WriteLine($"{_indents}Assign\tSource:{cmd.Source.Name},Recipient:{cmd.Recipient.Name}");
        }

        public void Execute(LessThan cmd)
        {
            _file.WriteLine($"{_indents}LessThan\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Different cmd)
        {
            _file.WriteLine($"{_indents}Different\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(LessOrEqualThan cmd)
        {
            _file.WriteLine($"{_indents}LessOrEqual\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Or cmd)
        {
            _file.WriteLine($"{_indents}Or\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(PushDefaults cmd)
        {
            _file.WriteLine($"{_indents}PushDefaults\tScope:{cmd.Scope.Name}");
        }

        public void Execute(Read cmd)
        {
            _file.WriteLine($"{_indents}Read\tResult:{cmd.Result.Name}");
        }

        public void Execute(Print cmd)
        {
            _file.WriteLine($"{_indents}Print\tParams:{string.Join(", ",cmd.Values.Select(v => v.Name))}");
        }

        public void Execute(AssignIndex cmd)
        {
            _file.WriteLine($"{_indents}AssignIndex\tIndexes:{string.Join(", ", cmd.Indexes.Select(v => v.Name))};Array:{cmd.Array.Name}");
        }

        public void Execute(CallFunction cmd)
        {
            _file.WriteLine($"{_indents}CallFunction\tFunction:{cmd.Function.Name};Parameters:{string.Join(", ", cmd.Parameters.Select(v => v.Name))};Result:{(cmd.Result != null ? cmd.Result.Name : "")}");
            if (_processedFunctions.Contains(cmd.Function)) return;
            _processedFunctions.Add(cmd.Function);
            _pendingFunctions.Add(cmd.Function);
        }

        public void Execute(Random cmd)
        {
            _file.WriteLine($"{_indents}Random\tResult:{cmd.Result.Name}");
        }

        public void Execute(While cmd)
        {
            _file.WriteLine($"{_indents}While\tResult:{cmd.Result}");
            _indents += "\t";
            _file.WriteLine($"{_indents}Expression:");
            _indents += "\t";
            cmd.Expression.ExecuteBy(this);
            _indents = _indents.Remove(0, 1);
            _file.WriteLine($"{_indents}WhileBlock:");
            _indents += "\t";
            cmd.WhileBlock.ExecuteBy(this);
            _indents = _indents.Remove(0, 2);
        }

        public void Execute(PopLocals cmd)
        {
            _file.WriteLine($"{_indents}PopLocals\tScope:{cmd.Scope.Name}");
        }

        public void Execute(Conditional cmd)
        {
            _file.WriteLine($"{_indents}Conditional\tCondition:{cmd.Condition.Name}");
            _indents += "\t";
            _file.WriteLine($"{_indents}If:");
            _indents += "\t";
            cmd.If.ExecuteBy(this);
            _indents = _indents.Remove(0, 1);
            _file.WriteLine($"{_indents}Else:");
            _indents += "\t";
            cmd.Else.ExecuteBy(this);
            _indents = _indents.Remove(0, 2);
        }

        public void Execute(And cmd)
        {
            _file.WriteLine($"{_indents}And\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(GreaterOrEqualThan cmd)
        {
            _file.WriteLine($"{_indents}GreaterOrEqual\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(GreaterThan cmd)
        {
            _file.WriteLine($"{_indents}Greater\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Equals cmd)
        {
            _file.WriteLine($"{_indents}Equals\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(AssignValue assignValue)
        {
            _file.WriteLine($"{_indents}AssignValue\tValue:{assignValue.Value.Name},Recipient:{assignValue.Recipient.Name}");
        }

        public void Execute(Modulo cmd)
        {
            _file.WriteLine($"{_indents}Module\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Divide cmd)
        {
            _file.WriteLine($"{_indents}Divide\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(Sum cmd)
        {
            _file.WriteLine($"{_indents}Sum\tOp1:{cmd.Op1.Name},Op2:{cmd.Op2.Name},Result:{cmd.Result.Name}");
        }

        public void Execute(CommandList commands)
        {
            foreach (var command in commands.Commands)
            {
                command.ExecuteBy(this);
            }
        }

        public void Execute(Line cmd)
        {
            _file.WriteLine($"{_indents}Line\tColor:{cmd.Color.Name},Thickness{cmd.Thickness.Name},X1:{cmd.X1.Name},Y1:{cmd.Y1.Name},X2:{cmd.X2.Name},Y2{cmd.Y2.Name}");
        }

        public void Execute(Arc cmd)
        {
            _file.WriteLine($"{_indents}Arc\tColor:{cmd.Color.Name},Thickness{cmd.Thickness.Name},X:{cmd.X.Name},Y:{cmd.Y.Name},Height:{cmd.Height.Name},Width:{cmd.Width.Name},StartAngle:{cmd.StartAngle.Name},FinalAngle:{cmd.FinalAngle.Name}");
        }

        public void Execute(Rectan cmd)
        {
            _file.WriteLine($"{_indents}Rectan\tX:{cmd.X.Name},Y:{cmd.Y.Name},Height:{cmd.Height.Name},Width:{cmd.Width.Name},LineColor:{cmd.LineColor.Name},BackgroundColor{cmd.BackgroundColor.Name}, Thickness:{cmd.Thickness.Name}");
        }

        public void Execute(Ellipse cmd)
        {
            _file.WriteLine($"{_indents}Elipse\tX:{cmd.X.Name},Y:{cmd.Y.Name},Height:{cmd.Height.Name},Width:{cmd.Width.Name},LineColor:{cmd.LineColor.Name},BackgroundColor{cmd.BackgroundColor.Name}, Thickness:{cmd.Thickness.Name}");
        }

        public void Execute(Triangle cmd)
        {
            _file.WriteLine($"{_indents}Triange\tX1:{cmd.X1.Name},Y1:{cmd.Y1.Name},X2:{cmd.X2.Name},Y2:{cmd.Y2.Name},X3:{cmd.X3.Name},Y3:{cmd.Y3.Name},LineColor:{cmd.LineColor.Name},BackgroundColor{cmd.BackgroundColor.Name}, Thickness:{cmd.Thickness.Name}");
        }

        public void Execute(Parse parse)
        {
            _file.WriteLine($"{_indents}Parse\tSource:{parse.Source.Name},Recipient:{parse.Recipient.Name}");
        }
    }
}
