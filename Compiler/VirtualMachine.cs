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
        private Pen colorPen = new Pen(Color.Black);

        /// <summary>
        /// Default color to fill a figure.
        /// </summary>
        private SolidBrush colorBack= new SolidBrush(Color.Black);

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

        public void Execute(Line cmd)
        {
            colorPen.Width = cmd.Thickness.Value;

            if (cmd.Color.Value.Equals("azul"))
            {
                colorPen.Color = Color.Blue;              
            }
            else if(cmd.Color.Value.Equals("amarillo"))
            {
                colorPen.Color = Color.Yellow;
            }
            else if (cmd.Color.Value.Equals("rojo"))
            {
                colorPen.Color = Color.Red;
            }
            else if (cmd.Color.Value.Equals("morado"))
            {
                colorPen.Color = Color.Purple;
            }
            else if (cmd.Color.Value.Equals("verde"))
            {
                colorPen.Color = Color.Green;
            }
            else if (cmd.Color.Value.Equals("naranja"))
            {
                colorPen.Color = Color.Orange;
            }
            else if (cmd.Color.Value.Equals("rosa"))
            {
                colorPen.Color = Color.Pink;
            }
            else if (cmd.Color.Value.Equals("cafe"))
            {
                colorPen.Color = Color.Brown;
            }
            else if (cmd.Color.Value.Equals("negro"))
            {
                colorPen.Color = Color.Black;
            }

            _graphics.DrawLine(colorPen, cmd.X1.Value, cmd.Y1.Value, cmd.X2.Value, cmd.Y2.Value);
        }

        public void Execute(Arc cmd)
        {
            colorPen.Width = cmd.Thickness.Value;

            if (cmd.Color.Value.Equals("azul"))
            {
                colorPen.Color = Color.Blue;
            }
            else if (cmd.Color.Value.Equals("amarillo"))
            {
                colorPen.Color = Color.Yellow;
            }
            else if (cmd.Color.Value.Equals("rojo"))
            {
                colorPen.Color = Color.Red;
            }
            else if (cmd.Color.Value.Equals("morado"))
            {
                colorPen.Color = Color.Purple;
            }
            else if (cmd.Color.Value.Equals("verde"))
            {
                colorPen.Color = Color.Green;
            }
            else if (cmd.Color.Value.Equals("naranja"))
            {
                colorPen.Color = Color.Orange;
            }
            else if (cmd.Color.Value.Equals("rosa"))
            {
                colorPen.Color = Color.Pink;
            }
            else if (cmd.Color.Value.Equals("cafe"))
            {
                colorPen.Color = Color.Brown;
            }
            else if (cmd.Color.Value.Equals("negro"))
            {
                colorPen.Color = Color.Black;
            }

            _graphics.DrawArc(colorPen, cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value, cmd.StartAngle.Value, cmd.FinalAngle.Value);
        }

        public void Execute(Rectan cmd)
        {
            colorPen.Width = cmd.Thickness.Value;

            //Line color
            if (cmd.LineColor.Value.Equals("azul"))
            {
                colorPen.Color = Color.Blue;
            }
            else if (cmd.LineColor.Value.Equals("amarillo"))
            {
                colorPen.Color = Color.Yellow;
            }
            else if (cmd.LineColor.Value.Equals("rojo"))
            {
                colorPen.Color = Color.Red;
            }
            else if (cmd.LineColor.Value.Equals("morado"))
            {
                colorPen.Color = Color.Purple;
            }
            else if (cmd.LineColor.Value.Equals("verde"))
            {
                colorPen.Color = Color.Green;
            }
            else if (cmd.LineColor.Value.Equals("naranja"))
            {
                colorPen.Color = Color.Orange;
            }
            else if (cmd.LineColor.Value.Equals("rosa"))
            {
                colorPen.Color = Color.Pink;
            }
            else if (cmd.LineColor.Value.Equals("cafe"))
            {
                colorPen.Color = Color.Brown;
            }
            else if (cmd.LineColor.Value.Equals("negro"))
            {
                colorPen.Color = Color.Black;
            }
            
            //Background color
            if (cmd.BackgroundColor.Value.Equals("azul"))
            {
                colorBack.Color = Color.Blue;
            }
            else if (cmd.BackgroundColor.Value.Equals("amarillo"))
            {
                colorBack.Color = Color.Yellow;
            }
            else if (cmd.BackgroundColor.Value.Equals("rojo"))
            {
                colorBack.Color = Color.Red;
            }
            else if (cmd.BackgroundColor.Value.Equals("morado"))
            {
                colorBack.Color = Color.Purple;
            }
            else if (cmd.BackgroundColor.Value.Equals("verde"))
            {
                colorBack.Color = Color.Green;
            }
            else if (cmd.BackgroundColor.Value.Equals("naranja"))
            {
                colorBack.Color = Color.Orange;
            }
            else if (cmd.BackgroundColor.Value.Equals("rosa"))
            {
                colorBack.Color = Color.Pink;
            }
            else if (cmd.BackgroundColor.Value.Equals("cafe"))
            {
                colorBack.Color = Color.Brown;
            }
            else if (cmd.BackgroundColor.Value.Equals("negro"))
            {
                colorBack.Color = Color.Black;
            }

            Rectangle rect = new Rectangle(cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value);

            _graphics.DrawRectangle(colorPen, rect);
            _graphics.FillRectangle(colorBack, rect);
        }

        public void Execute(Ellipse cmd)
        {
            colorPen.Width = cmd.Thickness.Value;

            //Line color
            if (cmd.LineColor.Value.Equals("azul"))
            {
                colorPen.Color = Color.Blue;
            }
            else if (cmd.LineColor.Value.Equals("amarillo"))
            {
                colorPen.Color = Color.Yellow;
            }
            else if (cmd.LineColor.Value.Equals("rojo"))
            {
                colorPen.Color = Color.Red;
            }
            else if (cmd.LineColor.Value.Equals("morado"))
            {
                colorPen.Color = Color.Purple;
            }
            else if (cmd.LineColor.Value.Equals("verde"))
            {
                colorPen.Color = Color.Green;
            }
            else if (cmd.LineColor.Value.Equals("naranja"))
            {
                colorPen.Color = Color.Orange;
            }
            else if (cmd.LineColor.Value.Equals("rosa"))
            {
                colorPen.Color = Color.Pink;
            }
            else if (cmd.LineColor.Value.Equals("cafe"))
            {
                colorPen.Color = Color.Brown;
            }
            else if (cmd.LineColor.Value.Equals("negro"))
            {
                colorPen.Color = Color.Black;
            }

            //Background color
            if (cmd.BackgroundColor.Value.Equals("azul"))
            {
                colorBack.Color = Color.Blue;
            }
            else if (cmd.BackgroundColor.Value.Equals("amarillo"))
            {
                colorBack.Color = Color.Yellow;
            }
            else if (cmd.BackgroundColor.Value.Equals("rojo"))
            {
                colorBack.Color = Color.Red;
            }
            else if (cmd.BackgroundColor.Value.Equals("morado"))
            {
                colorBack.Color = Color.Purple;
            }
            else if (cmd.BackgroundColor.Value.Equals("verde"))
            {
                colorBack.Color = Color.Green;
            }
            else if (cmd.BackgroundColor.Value.Equals("naranja"))
            {
                colorBack.Color = Color.Orange;
            }
            else if (cmd.BackgroundColor.Value.Equals("rosa"))
            {
                colorBack.Color = Color.Pink;
            }
            else if (cmd.BackgroundColor.Value.Equals("cafe"))
            {
                colorBack.Color = Color.Brown;
            }
            else if (cmd.BackgroundColor.Value.Equals("negro"))
            {
                colorBack.Color = Color.Black;
            }

            Rectangle rect = new Rectangle(cmd.X.Value, cmd.Y.Value, cmd.Width.Value, cmd.Height.Value);

            _graphics.DrawEllipse(colorPen, rect);
            _graphics.FillEllipse(colorBack, rect);
        }

        public void Execute(Triangle cmd)
        {
            colorPen.Width = cmd.Thickness.Value;

            PointF point1 = new PointF(cmd.X1.Value, cmd.Y1.Value);
            PointF point2 = new PointF(cmd.X2.Value, cmd.Y2.Value);
            PointF point3 = new PointF(cmd.X3.Value, cmd.Y3.Value);
                       
            PointF[] curvePoints =
             {
                 point1,
                 point2,
                 point3,
             };

            //Line color
            if (cmd.LineColor.Value.Equals("azul"))
            {
                colorPen.Color = Color.Blue;
            }
            else if (cmd.LineColor.Value.Equals("amarillo"))
            {
                colorPen.Color = Color.Yellow;
            }
            else if (cmd.LineColor.Value.Equals("rojo"))
            {
                colorPen.Color = Color.Red;
            }
            else if (cmd.LineColor.Value.Equals("morado"))
            {
                colorPen.Color = Color.Purple;
            }
            else if (cmd.LineColor.Value.Equals("verde"))
            {
                colorPen.Color = Color.Green;
            }
            else if (cmd.LineColor.Value.Equals("naranja"))
            {
                colorPen.Color = Color.Orange;
            }
            else if (cmd.LineColor.Value.Equals("rosa"))
            {
                colorPen.Color = Color.Pink;
            }
            else if (cmd.LineColor.Value.Equals("cafe"))
            {
                colorPen.Color = Color.Brown;
            }
            else if (cmd.LineColor.Value.Equals("negro"))
            {
                colorPen.Color = Color.Black;
            }

            //Background color
            if (cmd.BackgroundColor.Value.Equals("azul"))
            {
                colorBack.Color = Color.Blue;
            }
            else if (cmd.BackgroundColor.Value.Equals("amarillo"))
            {
                colorBack.Color = Color.Yellow;
            }
            else if (cmd.BackgroundColor.Value.Equals("rojo"))
            {
                colorBack.Color = Color.Red;
            }
            else if (cmd.BackgroundColor.Value.Equals("morado"))
            {
                colorBack.Color = Color.Purple;
            }
            else if (cmd.BackgroundColor.Value.Equals("verde"))
            {
                colorBack.Color = Color.Green;
            }
            else if (cmd.BackgroundColor.Value.Equals("naranja"))
            {
                colorBack.Color = Color.Orange;
            }
            else if (cmd.BackgroundColor.Value.Equals("rosa"))
            {
                colorBack.Color = Color.Pink;
            }
            else if (cmd.BackgroundColor.Value.Equals("cafe"))
            {
                colorBack.Color = Color.Brown;
            }
            else if (cmd.BackgroundColor.Value.Equals("negro"))
            {
                colorBack.Color = Color.Black;
            }

            _graphics.DrawPolygon(colorPen, curvePoints);
            _graphics.FillPolygon(colorBack, curvePoints);
        }

            /*
            public void Execute(DrawLine cmd)
            {
               _graphics.DrawLine(new Pen(Color.Black, 3), cmd.x1.Value, cmd.y1.Value, cmd.x2.Value, cmd.y2.Value );
            }
            */
        }
}
