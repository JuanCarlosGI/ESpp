using System.Collections.Generic;
using System.Linq;



using System;

namespace Coco_R {



public partial class Parser {
	public const int _EOF = 0;
	public const int _id = 1;
	public const int _ctestr = 2;
	public const int _cteent = 3;
	public const int _ctedbl = 4;
	public const int _lpar = 5;
	public const int maxT = 47;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void ESpp() {
		Program();
	}

	void Program() {
		while (StartOf(1)) {
			if (DoubleFollowedByLPar()) {
				DeclaracionFunc();
			} else {
				Declaracion();
			}
		}
		CheckFunctionsNoBody(); 
		Main();
	}

	void DeclaracionFunc() {
		Type funType = Type.Error; 
		if (StartOf(2)) {
			Tipo(out funType);
		} else if (la.kind == 6) {
			Get();
			funType = Type.Rutina; 
		} else SynErr(48);
		Expect(1);
		var funName = t.val; 
		Expect(5);
		var vars = new List<Variable>(); 
		if (StartOf(2)) {
			Type tipo; 
			Tipo(out tipo);
			Expect(1);
			vars.Add(new Variable(){Name=t.val, Type=tipo}); 
			while (la.kind == 7) {
				Get();
				Tipo(out tipo);
				Expect(1);
				vars.Add(new Variable(){Name=t.val, Type=tipo}); 
			}
		}
		Expect(8);
		AddFunction(funName, funType, vars); 
		if (la.kind == 17) {
			DirectValueSymbol returns; 
			Bloque(funName, vars.ToArray(), funType != Type.Rutina, out returns);
			AddReturns(funName, returns); LinkFunctionBody(funName); 
		} else if (la.kind == 9) {
			Get();
		} else SynErr(49);
	}

	void Declaracion() {
		Type tipo; bool isArr = false; List<int> sizes = null; 
		Tipo(out tipo);
		if (la.kind == 15) {
			TipoArr(out sizes);
			isArr = true; 
		}
		Expect(1);
		AddVariable(t.val, tipo, isArr, sizes); 
		while (la.kind == 7) {
			Get();
			Expect(1);
			AddVariable(t.val, tipo, isArr, sizes); 
		}
		Expect(9);
	}

	void Main() {
		DirectValueSymbol dummy; 
		Expect(10);
		Bloque("main", new Variable[]{}, false, out dummy);
	}

	void Tipo(out Type tipo) {
		Type tipoAux = Type.Error; 
		if (la.kind == 11) {
			Get();
			tipoAux = Type.Entero; 
		} else if (la.kind == 12) {
			Get();
			tipoAux = Type.Decimal; 
		} else if (la.kind == 13) {
			Get();
			tipoAux = Type.Booleano; 
		} else if (la.kind == 14) {
			Get();
			tipoAux = Type.Cadena; 
		} else SynErr(50);
		tipo = tipoAux; 
	}

	void Bloque(string name, Variable[] parameters, bool isFunction, out DirectValueSymbol returns) {
		Expect(17);
		CreateNewScope(name, new List<Variable>(parameters)); DoPushDefaults(); 
		returns = null; 
		while (StartOf(3)) {
			if (DoubleFollowedByLPar()) {
				DeclaracionFunc();
			} else if (StartOf(2)) {
				Declaracion();
			} else if (la.kind == 21) {
				Condicion();
			} else if (la.kind == 23) {
				Ciclo();
			} else if (la.kind == 25) {
				DrawLine();
			} else if (la.kind == 26) {
				DrawArc();
			} else if (la.kind == 27) {
				DrawRec();
			} else if (la.kind == 28) {
				DrawEllipse();
			} else if (la.kind == 29) {
				DrawTriangle();
			} else if (la.kind == 24) {
				Impresion();
			} else if (FollowedByLPar()) {
				Function function; List<DirectValueSymbol> paras; 
				Funcion(out function, out paras);
				Expect(9);
				DoRoutine(function, paras); 
			} else {
				Asignacion();
			}
		}
		CheckFunctionsNoBody(); var hasReturn = false; 
		if (la.kind == 18) {
			Get();
			Expresion();
			Expect(9);
			hasReturn = true; returns = _symbolStack.Pop(); _currentScope.Returns = returns; 
		}
		Expect(19);
		ValidateHasReturn(isFunction, hasReturn); DoPopLocals(); _currentScope = _currentScope.Parent; 
	}

	void TipoArr(out List<int> lengths) {
		lengths = new List<int>(); 
		Expect(15);
		Expect(3);
		lengths.Add(int.Parse(t.val)); 
		while (la.kind == 7) {
			Get();
			Expect(3);
			lengths.Add(int.Parse(t.val)); 
		}
		Expect(16);
	}

	void Condicion() {
		Expect(21);
		Expect(5);
		Expresion();
		Expect(8);
		var condition = _symbolStack.Pop(); DirectValueSymbol returnsDummy; 
		Bloque("if", new Variable[]{}, false, out returnsDummy);
		var ifBlock = _currentScope.Children.Last().CommandList; CommandList elseBlock = null; 
		if (la.kind == 22) {
			Get();
			Bloque("else", new Variable[]{}, false, out returnsDummy);
			elseBlock = _currentScope.Children.Last().CommandList; 
		}
		DoIfElse(condition, ifBlock, elseBlock); 
	}

	void Ciclo() {
		Expect(23);
		Expect(5);
		CreateNewScope("Expression", new List<Variable>());  DirectValueSymbol returnsDummy; 
		Expresion();
		var expression = _currentScope.CommandList; var result = _symbolStack.Pop(); 
		Expect(8);
		_currentScope = _currentScope.Parent; 
		Bloque("while", new Variable[]{}, false, out returnsDummy);
		var whileBlock = _currentScope.Children.Last().CommandList; DoWhile(expression, result, whileBlock); 
	}

	void DrawLine() {
		Expect(25);
		Expect(5);
		Expresion();
		var lineColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var thickness = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x1 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y1 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x2 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y2 = _symbolStack.Pop(); 
		Expect(8);
		Expect(9);
		DoLine(lineColor, thickness, x1, y1, x2, y2); 
	}

	void DrawArc() {
		Expect(26);
		Expect(5);
		Expresion();
		var lineColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var thickness = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var width = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var height = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var startAngle = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var finalAngle = _symbolStack.Pop(); 
		Expect(8);
		Expect(9);
		DoArc(lineColor, thickness, x, y, width, height, startAngle, finalAngle); 
	}

	void DrawRec() {
		Expect(27);
		Expect(5);
		Expresion();
		var backgroundColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var lineColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var thickness = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var width = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var height = _symbolStack.Pop(); 
		Expect(8);
		Expect(9);
		DoRectangle(backgroundColor, lineColor, thickness, x, y, width, height); 
	}

	void DrawEllipse() {
		Expect(28);
		Expect(5);
		Expresion();
		var backgroundColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var lineColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var thickness = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var width = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var height = _symbolStack.Pop(); 
		Expect(8);
		Expect(9);
		DoEllipse(backgroundColor, lineColor, thickness, x, y, width, height); 
	}

	void DrawTriangle() {
		Expect(29);
		Expect(5);
		Expresion();
		var backgroundColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var lineColor = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var thickness = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x1 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y1 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x2 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y2 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var x3 = _symbolStack.Pop(); 
		Expect(7);
		Expresion();
		var y3 = _symbolStack.Pop(); 
		Expect(8);
		Expect(9);
		DoTriangle(backgroundColor, lineColor, thickness, x1, y1, x2, y2, x3, y3); 
	}

	void Impresion() {
		Expect(24);
		Expect(5);
		var expressions = new List<DirectValueSymbol>(); 
		Expresion();
		expressions.Add(_symbolStack.Pop()); 
		while (la.kind == 7) {
			Get();
			Expresion();
			expressions.Add(_symbolStack.Pop()); 
		}
		Expect(8);
		Expect(9);
		DoPrint(expressions); 
	}

	void Funcion(out Function function, out List<DirectValueSymbol> parameters ) {
		Expect(1);
		string name = t.val; CheckFunctionExists(name); 
		Expect(5);
		parameters = new List<DirectValueSymbol>(); 
		if (StartOf(4)) {
			Expresion();
			parameters.Add(_symbolStack.Pop()); 
			while (la.kind == 7) {
				Get();
				Expresion();
				parameters.Add(_symbolStack.Pop()); 
			}
		}
		Expect(8);
		CheckParamAmount(name, parameters.Count); 
		function = _currentScope.Search(name) as Function; 
	}

	void Asignacion() {
		Variable variable; List<DirectValueSymbol> indexes = new List<DirectValueSymbol>(); 
		Variable(out variable, out indexes);
		_symbolStack.Push(variable); 
		Expect(20);
		Expresion();
		Expect(9);
		if(variable is VariableArray) DoAssignIndex((VariableArray)variable, indexes); DoAssign(); 
	}

	void Expresion() {
		Exp();
		while (la.kind == 30 || la.kind == 31) {
			if (la.kind == 30) {
				Get();
				_operatorStack.Push(Operator.And); 
			} else {
				Get();
				_operatorStack.Push(Operator.Or); 
			}
			Exp();
			DoPendingLogical(); 
		}
	}

	void Variable(out Variable variable, out List<DirectValueSymbol> indexes ) {
		indexes = new List<DirectValueSymbol>(); 
		Expect(1);
		string name = t.val; CheckVariableExists(name); var symbol = _currentScope.Search(name); variable = symbol as Variable; 
		if (la.kind == 15) {
			Get();
			Expresion();
			indexes.Add(_symbolStack.Pop() as DirectValueSymbol); 
			while (la.kind == 7) {
				Get();
				Expresion();
				indexes.Add(_symbolStack.Pop() as DirectValueSymbol); 
			}
			Expect(16);
			CheckIsArray(name); VariableArray array =(symbol as VariableArray); DoAssignIndex(array,indexes); variable = array; 
		}
	}

	void Exp() {
		Expt();
		if (StartOf(5)) {
			switch (la.kind) {
			case 32: {
				Get();
				_operatorStack.Push(Operator.GreaterThan); 
				break;
			}
			case 33: {
				Get();
				_operatorStack.Push(Operator.LessThan); 
				break;
			}
			case 34: {
				Get();
				_operatorStack.Push(Operator.GreaterEqual); 
				break;
			}
			case 35: {
				Get();
				_operatorStack.Push(Operator.LessEqual); 
				break;
			}
			case 36: {
				Get();
				_operatorStack.Push(Operator.Different); 
				break;
			}
			case 37: {
				Get();
				_operatorStack.Push(Operator.Equality); 
				break;
			}
			}
			Expt();
			DoPendingRelational(); 
		}
	}

	void Expt() {
		Termino();
		while (la.kind == 38 || la.kind == 39) {
			if (la.kind == 38) {
				Get();
				_operatorStack.Push(Operator.Sum); 
			} else {
				Get();
				_operatorStack.Push(Operator.Minus); 
			}
			Termino();
			DoPendingSum(); 
		}
	}

	void Termino() {
		Factor();
		while (la.kind == 40 || la.kind == 41 || la.kind == 42) {
			if (la.kind == 40) {
				Get();
				_operatorStack.Push(Operator.Multiply); 
			} else if (la.kind == 41) {
				Get();
				_operatorStack.Push(Operator.Divide); 
			} else {
				Get();
				_operatorStack.Push(Operator.Modulo); 
			}
			Factor();
			DoPendingMultiplication(); 
		}
	}

	void Factor() {
		var negative = false; 
		if (la.kind == 38 || la.kind == 39) {
			if (la.kind == 38) {
				Get();
			} else {
				Get();
				negative = true; 
			}
		}
		if (la.kind == 5) {
			Get();
			_operatorStack.Push(Operator.FakeLimit); 
			Expresion();
			Expect(8);
			_operatorStack.Pop(); 
		} else if (StartOf(6)) {
			DirectValueSymbol symbol; 
			Constante(out symbol);
			_symbolStack.Push(symbol); 
		} else SynErr(51);
		if(negative) DoNegative(); 
	}

	void Constante(out DirectValueSymbol sym) {
		sym = null; 
		if (la.kind == 3) {
			Get();
			sym = _constBuilder.IntConstant(t.val); 
		} else if (la.kind == 4) {
			Get();
			sym = _constBuilder.DecConstant(t.val); 
		} else if (la.kind == 45 || la.kind == 46) {
			Ctebol();
			sym = _constBuilder.BoolConstant(t.val); 
		} else if (la.kind == 2) {
			Get();
			sym = _constBuilder.StrConstant(t.val); 
		} else if (la.kind == 43) {
			Aleatorio();
			sym = _varBuilder.NewVariable(Type.Decimal); DoRandom(sym); _currentScope.Add(sym); 
		} else if (la.kind == 44) {
			Lectura();
			sym = _varBuilder.NewVariable(Type.Cadena); DoRead(sym); _currentScope.Add(sym); 
		} else if (FollowedByLPar()) {
			Function function; List<DirectValueSymbol> parameters;
			Funcion(out function, out parameters);
			var result = _varBuilder.NewVariable(function.Type); DoFunction(function, parameters, result); sym = result; _currentScope.Add(sym); 
		} else if (la.kind == 1) {
			Variable variable; List<DirectValueSymbol> indexes; 
			Variable(out variable, out indexes);
			sym = variable; if(variable is VariableArray) { sym = _varBuilder.NewVariable(variable.Type); DoGetArrayValue(variable, sym); } 
		} else SynErr(52);
	}

	void Ctebol() {
		if (la.kind == 45) {
			Get();
		} else if (la.kind == 46) {
			Get();
		} else SynErr(53);
	}

	void Aleatorio() {
		Expect(43);
		Expect(5);
		Expect(8);
	}

	void Lectura() {
		Expect(44);
		Expect(5);
		Expect(8);
	}



	public CommandList Parse() {
		la = new Token();
		la.val = "";		
		Get();
		ESpp();
		Expect(0);

		var main = _currentScope.SearchForFunctionScope("main");
		return main.CommandList;
	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_T,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_T,_x,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _x,_x,_x,_T, _T,_T,_T,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "id expected"; break;
			case 2: s = "ctestr expected"; break;
			case 3: s = "cteent expected"; break;
			case 4: s = "ctedbl expected"; break;
			case 5: s = "lpar expected"; break;
			case 6: s = "\"rutina\" expected"; break;
			case 7: s = "\",\" expected"; break;
			case 8: s = "\")\" expected"; break;
			case 9: s = "\";\" expected"; break;
			case 10: s = "\"main\" expected"; break;
			case 11: s = "\"entero\" expected"; break;
			case 12: s = "\"decimal\" expected"; break;
			case 13: s = "\"booleano\" expected"; break;
			case 14: s = "\"cadena\" expected"; break;
			case 15: s = "\"[\" expected"; break;
			case 16: s = "\"]\" expected"; break;
			case 17: s = "\"{\" expected"; break;
			case 18: s = "\"regresa\" expected"; break;
			case 19: s = "\"}\" expected"; break;
			case 20: s = "\"=\" expected"; break;
			case 21: s = "\"si\" expected"; break;
			case 22: s = "\"sino\" expected"; break;
			case 23: s = "\"mientras\" expected"; break;
			case 24: s = "\"imprimir\" expected"; break;
			case 25: s = "\"dibujarLinea\" expected"; break;
			case 26: s = "\"dibujarArco\" expected"; break;
			case 27: s = "\"dibujarRectangulo\" expected"; break;
			case 28: s = "\"dibujarElipse\" expected"; break;
			case 29: s = "\"dibujarTriangulo\" expected"; break;
			case 30: s = "\"&&\" expected"; break;
			case 31: s = "\"||\" expected"; break;
			case 32: s = "\">\" expected"; break;
			case 33: s = "\"<\" expected"; break;
			case 34: s = "\">=\" expected"; break;
			case 35: s = "\"<=\" expected"; break;
			case 36: s = "\"<>\" expected"; break;
			case 37: s = "\"==\" expected"; break;
			case 38: s = "\"+\" expected"; break;
			case 39: s = "\"-\" expected"; break;
			case 40: s = "\"*\" expected"; break;
			case 41: s = "\"/\" expected"; break;
			case 42: s = "\"%\" expected"; break;
			case 43: s = "\"aleatorio\" expected"; break;
			case 44: s = "\"lectura\" expected"; break;
			case 45: s = "\"verdadero\" expected"; break;
			case 46: s = "\"falso\" expected"; break;
			case 47: s = "??? expected"; break;
			case 48: s = "invalid DeclaracionFunc"; break;
			case 49: s = "invalid DeclaracionFunc"; break;
			case 50: s = "invalid Tipo"; break;
			case 51: s = "invalid Factor"; break;
			case 52: s = "invalid Constante"; break;
			case 53: s = "invalid Ctebol"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}