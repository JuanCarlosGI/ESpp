using System;
using System.Collections.Generic;
using System.Drawing;
using Coco_R;
using Random = Coco_R.Random;
using Type = Coco_R.Type;

namespace Compiler
{
    public class VirtualMachine : IVirtualMachine
    {
        private readonly System.Random _rand = new System.Random();

        /// <summary>
        /// Gets the resulting image from the virtual machine.
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// The graphics object from the image.
        /// </summary>
        private readonly Graphics _graphics;

        /// <summary>
        /// Default color of the Pen.
        /// </summary>
        private readonly Pen _colorPen = new Pen(Color.Black);

        /// <summary>
        /// Default color to fill a figure.
        /// </summary>
        private  readonly SolidBrush _colorBack = new SolidBrush(Color.Black);

        /// <summary>
        /// Object to parse color strings.
        /// </summary>
        private readonly ColorParser _colorParser = new ColorParser();

        public VirtualMachine()
        {
            Image = new Bitmap(1000, 1000);
             _graphics = Graphics.FromImage(Image);
        }

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

        public void Execute(AssignValue cmd)
        {
            switch (cmd.Recipient.Type)
            {
                case Type.Entero:
                    cmd.Recipient.Value = (int)cmd.Value;
                    break;
                case Type.Decimal:
                    cmd.Recipient.Value = (double)cmd.Value;
                    break;
                case Type.Booleano:
                    cmd.Recipient.Value = (bool)cmd.Value;
                    break;
                case Type.Cadena:
                    cmd.Recipient.Value = (string)cmd.Value;
                    break;
                default:
                    cmd.Recipient.Value = cmd.Value;
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
            cmd.Array.Indexes = cmd.Indexes;
        }

        public void Execute(CallFunction cmd)
        {
            var commands = cmd.Function.CommandList;

            if (commands == null) return;

            // Remove and save previous assign value commands
            var stack = new Stack<AssignValue>();
            while (commands.Commands[1] is AssignValue)
            {
                stack.Push((AssignValue)commands.Commands[1]);
                commands.Commands.RemoveAt(1);
            }

            // Add my assign value commands.
            for (var para = cmd.Parameters.Count - 1; para >= 0; para--)
            {
                var parameter = cmd.Function.Parameters[para];
                var newCmd = new AssignValue
                {
                    Recipient = parameter,
                    Value = cmd.Parameters[para].Value
                };
                commands.Commands.Insert(1, newCmd);
            }

            Execute(commands);

            // Remove my assign value commands
            for (var para = cmd.Parameters.Count - 1; para >= 0; para--)
            {
                commands.Commands.RemoveAt(1);
            }

            // Restor previous assign value commands.
            while (stack.Count != 0)
            {
                commands.Commands.Insert(1, stack.Pop());
            }

            if (cmd.Function.Type != Type.Rutina)
            {
                var aux = cmd.Function.Returns.Value;
                if (!(cmd.Function.Returns is Constant)) cmd.Function.Returns.Unroll();
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
            for (var i = 0; i < commands.Commands.Count; i++)
            {
                var command = commands.Commands[i];
                command.ExecuteBy(this);
            }
        }

        public void Execute(Line cmd)
        {
            _colorPen.Width = cmd.Thickness.Value;

            _colorPen.Color = _colorParser.Parse(cmd.Color.Value);

            _graphics.DrawLine(_colorPen, cmd.X1.Value, cmd.Y1.Value, cmd.X2.Value, cmd.Y2.Value);
        }

        public void Execute(Arc cmd)
        {
            _colorPen.Width = cmd.Thickness.Value;

            _colorPen.Color = _colorParser.Parse(cmd.Color.Value);

            _graphics.DrawArc(_colorPen, cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value, cmd.StartAngle.Value, cmd.FinalAngle.Value);
        }

        public void Execute(Rectan cmd)
        {
            _colorPen.Width = cmd.Thickness.Value;

            _colorPen.Color = _colorParser.Parse(cmd.LineColor.Value);
            _colorBack.Color = _colorParser.Parse(cmd.BackgroundColor.Value);

            var rect = new Rectangle(cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value);

            _graphics.DrawRectangle(_colorPen, rect);
            _graphics.FillRectangle(_colorBack, rect);
        }

        public void Execute(Ellipse cmd)
        {
            _colorPen.Width = cmd.Thickness.Value;

            _colorPen.Color = _colorParser.Parse(cmd.LineColor.Value);
            _colorBack.Color = _colorParser.Parse(cmd.BackgroundColor.Value);

            var rect = new Rectangle(cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value);

            _graphics.DrawEllipse(_colorPen, rect);
            _graphics.FillEllipse(_colorBack, rect);
        }

        public void Execute(Triangle cmd)
        {
            _colorPen.Width = cmd.Thickness.Value;

            var point1 = new PointF((float)cmd.X1.Value, (float)cmd.Y1.Value);
            var point2 = new PointF((float)cmd.X2.Value, (float)cmd.Y2.Value);
            var point3 = new PointF((float)cmd.X3.Value, (float)cmd.Y3.Value);
                       
            PointF[] curvePoints =
             {
                 point1,
                 point2,
                 point3
             };

            _colorPen.Color = _colorParser.Parse(cmd.LineColor.Value);
            _colorBack.Color = _colorParser.Parse(cmd.BackgroundColor.Value);

            _graphics.DrawPolygon(_colorPen, curvePoints);
            _graphics.FillPolygon(_colorBack, curvePoints);
        }    
    }
}
