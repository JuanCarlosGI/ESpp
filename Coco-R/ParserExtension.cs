using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coco_R
{
    public partial class Parser
    {
        ConstantBuilder constBuilder = new EsConstantBuilder();
        CodeBlock currentCodeBlock = new CodeBlock(null, "global");
        Stack<DirectValueSymbol> symbolStack = new Stack<DirectValueSymbol>();
        Stack<Operator> operatorStack = new Stack<Operator>();

        int tempCounter = 1;

        bool FollowedByLPar()
        {
            return scanner.Peek().kind == _lpar;
        }

        void addVariable(string name, Type tipo, bool isArr, int size)
        {
            if (!currentCodeBlock.ExistsInScope(name))
            {
                if (isArr)
                {
                    Symbol symbol = new VariableArray(size)
                    {
                        Name = name,
                        Type = tipo
                    };
                    currentCodeBlock.Add(symbol);
                }
                else
                {
                    Symbol symbol = new Variable
                    {
                        Name = name,
                        Type = tipo,
                        Value = EsConstantBuilder.DefaultValue(tipo)
                    };
                    currentCodeBlock.Add(symbol);
                }
            }
            else
                SemErr($"El nombre {name} ya ha sido declarado en este scope.");
        }

        void checkVariableExists(string name)
        {
            var search = currentCodeBlock.Search(name);
            if (search == null)
                SemErr($"La variable {name} no ha sido declarada.");
            else if (!(search is Variable))
                SemErr($"El nombre {name} no se refiere a una variable.");
        }

        void checkFunctionExists(string name)
        {
            var search = currentCodeBlock.Search(name);
            if (search == null)
                SemErr($"La función {name} no ha sido declarada.");
            else if (!(search is Function))
                SemErr($"El nombre {name} no se refiere a una funcion.");
        }

        void checkIsArray(string name)
        {
            var symbol = currentCodeBlock.Search(name) as VariableArray;
            if (symbol == null)
                SemErr($"La variable {name} no es un arreglo.");
        }

        void createNewSymbolTable(string name, List<Variable> parameters)
        {
            var newTable = new CodeBlock(currentCodeBlock, name);
            currentCodeBlock.Children.Add(newTable);
            currentCodeBlock = newTable;
            addParameters(parameters.ToArray());
        }

        void addParameters(Variable[] parameters)
        {
            foreach (var variable in parameters)
            {
                currentCodeBlock.Add(variable);
            }
        }

        void addReturns(string name, DirectValueSymbol returns)
        {
            var function = currentCodeBlock.Search(name) as Function;
            function.Returns = returns;
        }

        void addFunction(string name, Type tipo, List<Variable> parameters)
        {
            if (!currentCodeBlock.ExistsInScope(name))
            {
                var fun = new Function
                {
                    Name = name,
                    Type = tipo,
                    Parameters = parameters
                };

                currentCodeBlock.Add(fun);
            }
            else
            {
                SemErr($"El nombre {name} ya ha sido declarado en este scope.");
            }
        }

        void checkParamAmount(string name, int amount)
        {
            var fun = currentCodeBlock.Search(name) as Function;
            if (fun == null || fun.Parameters.Count != amount)
            {
                SemErr($"La funcion {name} no tiene {amount} parametros.");
            }
        }

        private bool CheckTypeMismatch(DirectValueSymbol left, DirectValueSymbol right, Operator op, out Type type)
        {
            type = cubo[(int)left.Type, (int)right.Type, (int)op];
            if (type == Type.Error)
            {
                SemErr("Type Mismatch");
                return false;
            }

            return true;
        }

        string nextTempName()
        {
            return $"temp{tempCounter++}";
        }

        void doPendingSum()
        {
            var op2 = symbolStack.Pop();
            var op1 = symbolStack.Pop();
            var oper = operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1,op2,oper,out type))
            {
                var result = new Constant { Type = type, Name = nextTempName() };
                Command cmd = null;
                switch (oper)
                {
                    case Operator.Sum:
                        cmd = new Sum { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Minus:
                        cmd = new Subtract { Op1 = op1, Op2 = op2, Result = result };
                        break;
                }

                symbolStack.Push(result);
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }

        void doPendingMultiplication()
        {
            var op2 = symbolStack.Pop();
            var op1 = symbolStack.Pop();
            var oper = operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = new Constant { Type = type, Name = nextTempName() };
                Command cmd = null;
                switch (oper)
                {
                    case Operator.Multiply:
                        cmd = new Multiply { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Divide:
                        cmd = new Divide { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Modulo:
                        cmd = new Modulo { Op1 = op1, Op2 = op2, Result = result };
                        break;
                }

                symbolStack.Push(result);
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }

        void doPendingRelational()
        {
            var op2 = symbolStack.Pop();
            var op1 = symbolStack.Pop();
            var oper = operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = new Constant { Type = type, Name = nextTempName() };
                Command cmd = null;
                switch (oper)
                {
                    case Operator.LessThan:
                        cmd = new LessThan { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.GreaterThan:
                        cmd = new GreaterThan { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Equality:
                        cmd = new Equals { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.LessEqual:
                        cmd = new LessOrEqualThan { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.GreaterEqual:
                        cmd = new GreaterOrEqualThan { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Different:
                        cmd = new Different { Op1 = op1, Op2 = op2, Result = result };
                        break;
                }

                symbolStack.Push(result);
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }

        void doPendingLogical()
        {
            var op2 = symbolStack.Pop();
            var op1 = symbolStack.Pop();
            var oper = operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = new Constant { Type = type, Name = nextTempName() };
                Command cmd = null;
                switch (oper)
                {
                    case Operator.And:
                        cmd = new And { Op1 = op1, Op2 = op2, Result = result };
                        break;
                    case Operator.Or:
                        cmd = new Or { Op1 = op1, Op2 = op2, Result = result };
                        break;
                }

                symbolStack.Push(result);
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }

        void doAssign()
        {
            var op2 = symbolStack.Pop();
            var op1 = symbolStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, Operator.Asignation, out type))
            {
                var cmd = new Assign { Op1 = op1, Op2 = op2, Result = op1 };
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }

        void doIfElse(DirectValueSymbol condition, CommandList ifBlock, CommandList elseBlock)
        {
            var cmd = new Conditional
            {
                Condition = condition,
                If = ifBlock,
                Else = elseBlock
            };

            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doPushDefaults()
        { 
            var cmd = new PushDefaults
            {
                CodeBlock = currentCodeBlock
            };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doPopLocals()
        {
            var cmd = new PopLocals
            {
                CodeBlock = currentCodeBlock
            };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doRead(DirectValueSymbol result)
        {
            var cmd = new Read { Result = result };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doWhile(CommandList expression, DirectValueSymbol result, CommandList whileBlock)
        {
            if (result.Type != Type.Booleano)
            {
                SemErr("Type mismatch");
                return;
            }

            var cmd = new While { Expression = expression, WhileBlock = whileBlock, Result = result };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doRandom(DirectValueSymbol result)
        {
            var cmd = new Random { Result = result };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doPrint(List<DirectValueSymbol> values)
        {
            var cmd = new Print { Values = values };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doFunction(Function function, List<DirectValueSymbol> parameters, DirectValueSymbol result)
        {
            if (function == null)
            {
                SemErr("Function does not exist.");
                return;
            }
            if (function.Type == Type.Rutina)
            {
                SemErr("Function does not have a return value.");
                return;
            }
            if (function.Parameters.Count != parameters.Count)
            {
                SemErr("Wrong amount of arguments.");
                return;
            }

            for (int para = 0; para < parameters.Count; para++)
            {
                if (cubo[(int)function.Parameters[para].Type, (int)parameters[para].Type, (int)Operator.Asignation] == Type.Error)
                {
                    SemErr("Type mismatch");
                    return;
                }
            }

            var cmd = new CallFunction
            {
                Function = function,
                ScopeCalled = currentCodeBlock,
                Result = result,
                Parameters = parameters
            };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }

        void doRoutine(Function function, List<DirectValueSymbol> parameters)
        {
            if (function == null)
            {
                SemErr("Function does not exist.");
                return;
            }
            if (function.Type != Type.Rutina)
            {
                SemErr("Function is not a routine.");
                return;
            }
            if (function.Parameters.Count != parameters.Count)
            {
                SemErr("Wrong amount of arguments.");
                return;
            }

            for (int para = 0; para < parameters.Count; para++)
            {
                if (cubo[(int)function.Parameters[para].Type, (int)parameters[para].Type, (int)Operator.Asignation] == Type.Error)
                {
                    SemErr("Type mismatch");
                    return;
                }
            }

            var cmd = new CallFunction
            {
                Function = function,
                ScopeCalled = currentCodeBlock
            };
            currentCodeBlock.CommandList.Commands.Add(cmd);
        }
        
        void doAssignParameters(Variable[] parameters)
        {
            for(int para = parameters.Length -1; para >= 0; para--)
            {
                var parameter = parameters[para];
                var cmd = new AssignParam { Parameter = parameter };
                currentCodeBlock.CommandList.Commands.Add(cmd);
            }
        }
    }
}
