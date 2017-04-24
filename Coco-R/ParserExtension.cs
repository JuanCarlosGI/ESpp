using System.Collections.Generic;

namespace Coco_R
{
    public partial class Parser
    {
        /// <summary>
        /// Helper used to construct constants easilty.
        /// </summary>
        private readonly ConstantBuilder _constBuilder = 
            new ConstantBuilder();

        private readonly VariableBuilder _varBuilder = 
            new VariableBuilder();

        /// <summary>
        /// The current code block that the parser is in. At the end of the
        /// parsing process, it will be the global context.
        /// </summary>
        private Scope _currentScope = new Scope(null, "global");

        /// <summary>
        /// The symbol stack to be used during the parsing process.
        /// </summary>
        private readonly Stack<DirectValueSymbol> _symbolStack = 
            new Stack<DirectValueSymbol>();

        /// <summary>
        /// The operator stack to be used during the parsing process.
        /// </summary>
        private readonly Stack<Operator> _operatorStack = new Stack<Operator>();

        /// <summary>
        /// Determines whether the next token is a left parenthesis.
        /// </summary>
        /// <returns></returns>
        private bool FollowedByLPar()
        {
            return scanner.Peek().kind == _lpar;
        }

        /// <summary>
        /// Attempts to add a variable to the current code block hash, checking
        /// that it is a valid movement to make.
        /// </summary>
        /// <param name="name">The name of the variable to add.</param>
        /// <param name="tipo">The type of the variable to add.</param>
        /// <param name="isArr">Boolean determining if the variable is an array
        /// </param>
        /// <param name="size">In case of being an array, its size</param>
        private void AddVariable(string name, Type tipo, bool isArr, int size)
        {
            if (!_currentScope.ExistsInScope(name))
            {
                if (isArr)
                {
                    Symbol symbol = new VariableArray(size)
                    {
                        Name = name,
                        Type = tipo
                    };
                    _currentScope.Add(symbol);
                }
                else
                {
                    Symbol symbol = new Variable
                    {
                        Name = name,
                        Type = tipo,
                        Value = ConstantBuilder.DefaultValue(tipo)
                    };
                    _currentScope.Add(symbol);
                }
            }
            else
                SemErr($"El nombre {name} ya ha sido declarado en este scope.");
        }

        private void CheckVariableExists(string name)
        {
            var search = _currentScope.Search(name);
            if (search == null)
                SemErr($"La variable {name} no ha sido declarada.");
            else if (!(search is Variable))
                SemErr($"El nombre {name} no se refiere a una variable.");
        }

        private void CheckFunctionExists(string name)
        {
            var search = _currentScope.Search(name);
            if (search == null)
                SemErr($"La función {name} no ha sido declarada.");
            else if (!(search is Function))
                SemErr($"El nombre {name} no se refiere a una funcion.");
        }

        private void CheckIsArray(string name)
        {
            var symbol = _currentScope.Search(name) as VariableArray;
            if (symbol == null)
                SemErr($"La variable {name} no es un arreglo.");
        }

        private void CreateNewSymbolTable(string name, List<Variable> parameters)
        {
            var newTable = new Scope(_currentScope, name);
            _currentScope.Children.Add(newTable);
            _currentScope = newTable;
            AddParameters(parameters.ToArray());
        }

        private void AddParameters(Variable[] parameters)
        {
            foreach (var variable in parameters)
            {
                _currentScope.Add(variable);
            }
        }

        private void AddReturns(string name, DirectValueSymbol returns)
        {
            var function = _currentScope.Search(name) as Function;
            if (function != null) function.Returns = returns;
        }

        private void AddFunction(string name, Type tipo, List<Variable> parameters)
        {
            if (!_currentScope.ExistsInScope(name))
            {
                var fun = new Function
                {
                    Name = name,
                    Type = tipo,
                    Parameters = parameters
                };

                _currentScope.Add(fun);
            }
            else
            {
                SemErr($"El nombre {name} ya ha sido declarado en este scope.");
            }
        }

        private void LinkFunctionBody(string functionName)
        {
            var function = _currentScope.Search(functionName) as Function;
            var body = _currentScope.SearchForFunctionScope(functionName).CommandList;

            if (function != null) function.CommandList = body;
        }

        private void CheckParamAmount(string name, int amount)
        {
            var fun = _currentScope.Search(name) as Function;
            if (fun == null || fun.Parameters.Count != amount)
            {
                SemErr($"La funcion {name} no tiene {amount} parametros.");
            }
        }

        private bool CheckTypeMismatch(Symbol left, Symbol right, Operator op, out Type type)
        {
            type = cubo[(int)left.Type, (int)right.Type, (int)op];
            if (type == Type.Error)
            {
                SemErr("Type Mismatch");
                _symbolStack.Push(new Constant { Type = Type.Entero });
                return false;
            }

            return true;
        }
        
        private void DoPendingSum()
        {
            var op2 = _symbolStack.Pop();
            var op1 = _symbolStack.Pop();
            var oper = _operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1,op2,oper,out type))
            {
                var result = _varBuilder.NewVariable(type);
                _currentScope.Add(result);
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

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        private void DoPendingMultiplication()
        {
            var op2 = _symbolStack.Pop();
            var op1 = _symbolStack.Pop();
            var oper = _operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = _varBuilder.NewVariable(type);
                _currentScope.Add(result);
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

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        private void DoPendingRelational()
        {
            var op2 = _symbolStack.Pop();
            var op1 = _symbolStack.Pop();
            var oper = _operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = _varBuilder.NewVariable(type);
                _currentScope.Add(result);
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

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        private void DoPendingLogical()
        {
            var op2 = _symbolStack.Pop();
            var op1 = _symbolStack.Pop();
            var oper = _operatorStack.Pop();

            Type type;
            if (CheckTypeMismatch(op1, op2, oper, out type))
            {
                var result = _varBuilder.NewVariable(type);
                _currentScope.Add(result);
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

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        private void DoAssign()
        {
            var source = _symbolStack.Pop();
            var recipient = _symbolStack.Pop();

            Type type;
            if (CheckTypeMismatch(recipient, source, Operator.Asignation, out type))
            {
                var cmd = new Assign { Recipient = recipient, Source = source };
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        private void DoIfElse(DirectValueSymbol condition, CommandList ifBlock, CommandList elseBlock)
        {
            var cmd = new Conditional
            {
                Condition = condition,
                If = ifBlock,
                Else = elseBlock
            };

            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoPushDefaults()
        { 
            var cmd = new PushDefaults
            {
                Scope = _currentScope
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoAssignIndex(VariableArray array, DirectValueSymbol index)
        {
            var cmd = new AssignIndex
            {
                Array = array,
                Index = index
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoPopLocals()
        {
            var cmd = new PopLocals
            {
                Scope = _currentScope
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoRead(DirectValueSymbol result)
        {
            var cmd = new Read { Result = result };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoWhile(CommandList expression, DirectValueSymbol result, CommandList whileBlock)
        {
            if (result.Type != Type.Booleano)
            {
                SemErr("Type mismatch");
                return;
            }

            var cmd = new While { Expression = expression, WhileBlock = whileBlock, Result = result };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoRandom(DirectValueSymbol result)
        {
            var cmd = new Random { Result = result };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoPrint(List<DirectValueSymbol> values)
        {
            var cmd = new Print { Values = values };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoFunction(Function function, List<DirectValueSymbol> parameters, DirectValueSymbol result)
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

            for (var para = 0; para < parameters.Count; para++)
            {
                if (cubo[(int)function.Parameters[para].Type, (int)parameters[para].Type, (int)Operator.Asignation] == Type.Error)
                {
                    SemErr("Type mismatch");
                    return;
                }
            }

            result.Type = function.Type;

            var cmd = new CallFunction
            {
                Function = function,
                ScopeCalled = _currentScope,
                Result = result,
                Parameters = parameters
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        private void DoRoutine(Function function, List<DirectValueSymbol> parameters)
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

            for (var para = 0; para < parameters.Count; para++)
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
                ScopeCalled = _currentScope
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }
    }
}
