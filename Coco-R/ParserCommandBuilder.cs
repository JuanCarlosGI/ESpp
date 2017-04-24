 using System.Collections.Generic;

namespace Coco_R
{
    // This portion of the parser is in charge of building the commands.
    public partial class Parser
    {
        /// <summary>
        /// Builds a sum or suntract command and adds it to current scope.
        /// Type and operands are gotten from symbol stack.
        /// Result is pushed to the symbol stack.
        /// </summary>
        private void DoPendingSum()
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
                    case Operator.Sum:
                        cmd = new Sum {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Minus:
                        cmd = new Subtract {Op1 = op1, Op2 = op2, Result = result};
                        break;
                }

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        /// <summary>
        /// Builds a multiplication or division command and adds it to current 
        /// scope. 
        /// Type and operands are gotten from symbol stack.
        /// Result is pushed to the symbol stack.
        /// </summary>
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
                        cmd = new Multiply {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Divide:
                        cmd = new Divide {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Modulo:
                        cmd = new Modulo {Op1 = op1, Op2 = op2, Result = result};
                        break;
                }

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        /// <summary>
        /// Builds a command with a relational operation and adds it to current
        /// scope.
        /// Type and operands are gotten from symbol stack.
        /// Result is pushed to the symbol stack.
        /// </summary>
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
                        cmd = new LessThan {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.GreaterThan:
                        cmd = new GreaterThan {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Equality:
                        cmd = new Equals {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.LessEqual:
                        cmd = new LessOrEqualThan {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.GreaterEqual:
                        cmd = new GreaterOrEqualThan {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Different:
                        cmd = new Different {Op1 = op1, Op2 = op2, Result = result};
                        break;
                }

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        /// <summary>
        /// Builds a command with a logical operation and adds it to current
        /// scope.
        /// Type and operands are gotten from symbol stack.
        /// Result is pushed to the symbol stack.
        /// </summary>
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
                        cmd = new And {Op1 = op1, Op2 = op2, Result = result};
                        break;
                    case Operator.Or:
                        cmd = new Or {Op1 = op1, Op2 = op2, Result = result};
                        break;
                }

                _symbolStack.Push(result);
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        /// <summary>
        /// Builds an assign command and adds it to current scope.
        /// Type and operands are gotten from symbol stack.
        /// </summary>
        private void DoAssign()
        {
            var source = _symbolStack.Pop();
            var recipient = _symbolStack.Pop();

            Type type;
            if (CheckTypeMismatch(recipient, source, Operator.Asignation, out type))
            {
                var cmd = new Assign {Recipient = recipient, Source = source};
                _currentScope.CommandList.Commands.Add(cmd);
            }
        }

        /// <summary>
        /// Builds a conditional command and adds it to the current scope.
        /// </summary>
        /// <param name="condition">The symbol containing the condition value.
        /// </param>
        /// <param name="ifBlock">Commands executed if condition is met.</param>
        /// <param name="elseBlock">Commands to execute if false.</param>
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

        /// <summary>
        /// Builds a new command to push default values to scope.
        /// </summary>
        private void DoPushDefaults()
        {
            var cmd = new PushDefaults
            {
                Scope = _currentScope
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new cAssignIndex command
        /// </summary>
        /// <param name="array">Array to be assigned.</param>
        /// <param name="index">The index</param>
        private void DoAssignIndex(VariableArray array, DirectValueSymbol index)
        {
            var cmd = new AssignIndex
            {
                Array = array,
                Index = index
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new command to pop all local values from scope.
        /// </summary>
        private void DoPopLocals()
        {
            var cmd = new PopLocals
            {
                Scope = _currentScope
            };
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new Read command and adds it to current scope.
        /// </summary>
        /// <param name="result"></param>
        private void DoRead(DirectValueSymbol result)
        {
            var cmd = new Read {Result = result};
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new While command and adds it to current scope.
        /// </summary>
        /// <param name="expression">Commands to update the condition.</param>
        /// <param name="result">Symbol where result of condition is stored.</param>
        /// <param name="whileBlock">Commands to be executed in loop.</param>
        private void DoWhile(CommandList expression, DirectValueSymbol result, CommandList whileBlock)
        {
            if (result.Type != Type.Booleano)
            {
                SemErr("Type mismatch");
                return;
            }

            var cmd = new While {Expression = expression, WhileBlock = whileBlock, Result = result};
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new Random command and adds it to current scope.
        /// </summary>
        /// <param name="result">Symbol where result will be stored.</param>
        private void DoRandom(DirectValueSymbol result)
        {
            var cmd = new Random {Result = result};
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new Print command and adds it to current scope.
        /// </summary>
        /// <param name="values">Values to be printed.</param>
        private void DoPrint(List<DirectValueSymbol> values)
        {
            var cmd = new Print {Values = values};
            _currentScope.CommandList.Commands.Add(cmd);
        }

        /// <summary>
        /// Builds a new CallFunction command and adds it to current scope.
        /// </summary>
        /// <param name="function">Function to be called</param>
        /// <param name="parameters">Symbols that correspond to the parameters.
        /// </param>
        /// <param name="result">Symbol where result will be stored.</param>
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
                if (Cube[(int) function.Parameters[para].Type, (int) parameters[para].Type,
                        (int) Operator.Asignation] == Type.Error)
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

        /// <summary>
        /// Builds a new CallFunction command for a routine (no return value) 
        /// and adds it to current scope.
        /// </summary>
        /// <param name="function">Function to be called.</param>
        /// <param name="parameters">Symbols corresponding to the function's
        /// parameters.</param>
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
                if (Cube[(int) function.Parameters[para].Type, (int) parameters[para].Type,
                        (int) Operator.Asignation] == Type.Error)
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