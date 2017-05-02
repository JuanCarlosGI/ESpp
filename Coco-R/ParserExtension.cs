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

        /// <summary>
        /// Helper used to construct variables easily.
        /// </summary>
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
        /// <returns>If it is true</returns>
        private bool FollowedByLPar()
        {
            scanner.ResetPeek();
            return scanner.Peek().kind == _lpar;
        }

        /// <summary>
        /// Determines whether the next next token is a left parenthesis.
        /// </summary>
        /// <returns>If it is true</returns>
        private bool DoubleFollowedByLPar()
        {
            scanner.ResetPeek();
            scanner.Peek();
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
        /// <param name="sizes">In case of being an array, its size</param>
        private void AddVariable(string name, Type tipo, bool isArr, List<int> sizes)
        {
            if (!_currentScope.ExistsInScope(name))
            {
                if (isArr)
                {
                    Symbol symbol = new VariableArray(sizes)
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
                SemErr($"El nombre '{name}' ya ha sido declarado en este scope.");
        }

        /// <summary>
        /// Checks that a variable exists within the current available scopes.
        /// </summary>
        /// <param name="name">The name of a variable.</param>
        private void CheckVariableExists(string name)
        {
            var search = _currentScope.Search(name);
            if (search == null)
                SemErr($"La variable '{name}' no ha sido declarada.");
            else if (!(search is Variable))
                SemErr($"El nombre '{name}' no se refiere a una variable.");
        }

        /// <summary>
        /// Checks that a function exists within the current available scopes.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        private void CheckFunctionExists(string name)
        {
            var search = _currentScope.Search(name);
            if (search == null)
                SemErr($"La función '{name}' no ha sido declarada.");
            else if (!(search is Function))
                SemErr($"El nombre '{name}' no se refiere a una funcion.");
        }

        /// <summary>
        /// Checks that a variable array exists within the current available
        /// scopes.
        /// </summary>
        /// <param name="name">The name of the variable array.</param>
        private void CheckIsArray(string name)
        {
            var symbol = _currentScope.Search(name) as VariableArray;
            if (symbol == null)
                SemErr($"La variable '{name}' no es una estructura multidimensionada.");
        }

        /// <summary>
        /// Creates a new scope within the current scope, and adds all its
        /// parameters to its hash.
        /// </summary>
        /// <param name="name">The name of the new scope.</param>
        /// <param name="parameters">The list of parameters to be added.</param>
        private void CreateNewScope(string name, List<Variable> parameters)
        {
            var newScope = new Scope(_currentScope, name);
            _currentScope.Children.Add(newScope);
            _currentScope = newScope;
            AddParameters(parameters.ToArray());
        }

        /// <summary>
        /// Adds an array of parameters to the hash of the current scope.
        /// </summary>
        /// <param name="parameters"></param>
        private void AddParameters(Variable[] parameters)
        {
            foreach (var variable in parameters)
            {
                _currentScope.Add(variable);
            }
        }

        /// <summary>
        /// Updates a function object to include its return symbol.
        /// </summary>
        /// <param name="name">Name of the function to update.</param>
        /// <param name="returns">The symbol with the return value.</param>
        private void AddReturns(string name, DirectValueSymbol returns)
        {
            var function = _currentScope.Search(name) as Function;
            if (function != null) function.Returns = returns;
        }

        /// <summary>
        /// Adds a function to the current scope.
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="tipo">Return type of the function.</param>
        /// <param name="parameters">The parameters that the function will
        /// receive.</param>
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
                // Search for funtion in current scope, NOT in superior ones.
                var func = _currentScope.Search(name) as Function;
                if (func == null || func.CommandList != null)
                {
                    SemErr($"El nombre '{name}' ya ha sido declarado en este scope.");
                }
                else if (func.Type == tipo && parameters.Count == func.Parameters.Count)
                {
                    // Verify that all parameters have the same name and type
                    bool success = true;
                    for (var param = 0; param < parameters.Count; param++)
                    {
                        if (parameters[param].Type != func.Parameters[param].Type ||
                            parameters[param].Name != func.Parameters[param].Name)
                        {
                            SemErr($"La firma de '{name}' no coincide con una declarada anteriormente.");
                            success = false;
                            break;
                        }  
                    }

                    // Update reference to parameters.
                    if (success)
                    {
                        func.Parameters = parameters;
                    }
                    return;
                }
                SemErr($"La firma de '{name}' no coincide con una declarada anteriormente.");
            }
        }

        /// <summary>
        /// Links a function symbol with its body, by adding the command list.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        private void LinkFunctionBody(string functionName)
        {
            var function = _currentScope.ExistsInScope(functionName) ? _currentScope.Search(functionName) as Function : null;
            if (function == null) return;

            var body = _currentScope.SearchForFunctionScope(functionName).CommandList;
            function.CommandList = body;
        }

        /// <summary>
        /// Checks that a function has a certain amount of parameters.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        /// <param name="amount">Amount of parameters.</param>
        private void CheckParamAmount(string name, int amount)
        {
            var fun = _currentScope.Search(name) as Function;
            if (fun == null || fun.Parameters.Count != amount)
            {
                SemErr($"La función '{name}' no tiene {amount} parametros.");
            }
        }

        /// <summary>
        /// Determines whether the operation is valid and the return type of
        /// the operation given its operands.
        /// </summary>
        /// <param name="left">The operand on the left.</param>
        /// <param name="right">The operand on the right.</param>
        /// <param name="op">The operator</param>
        /// <param name="type">The resulting type.</param>
        /// <returns></returns>
        private bool CheckTypeMismatch(Symbol left, Symbol right, Operator op, out Type type)
        {
            type = _cube[(int)left.Type, (int)right.Type, (int)op];
            if (type != Type.Error) return true;

            SemErr("Error de tipos.");
            _symbolStack.Push(new Constant { Type = Type.Entero });
            return false;
        }

        /// <summary>
        /// Validates that a function has a retrun value and a routine doesn't.
        /// </summary>
        /// <param name="should">Value indicating whether its a function.</param>
        /// <param name="has">Value indicating whether it has a return.</param>
        private void ValidateHasReturn(bool should, bool has)
        {
            if (should)
            {
                if (!has)
                    SemErr("La función debe de tener un valor de retorno.");
            }
            else
            {
                if (has)
                    SemErr("Las rutinas no pueden tener valor de retorno.");
            }
        }

        /// <summary>
        /// Checks that no function is left without a body because of signatures.
        /// </summary>
        private void CheckFunctionsNoBody()
        {
            foreach (var symbol in _currentScope.Symbols)
            {
                var function = symbol as Function;
                if (function == null) continue;
                if (function.CommandList == null)
                {
                    SemErr($"No se declaró el cuerpo de la funcion '{function.Name}'");
                }
            }
        }
    }
}
