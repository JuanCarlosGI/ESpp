using System.Collections.Generic;



using System;

namespace Coco_R {



public class Parser {
	public const int _EOF = 0;
	public const int _id = 1;
	public const int _ctestr = 2;
	public const int _cteent = 3;
	public const int _ctedbl = 4;
	public const int _lpar = 5;
	public const int maxT = 44;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public SymbolTable currentSymbolTable;

bool FollowedByLPar() {
 return scanner.Peek().kind == _lpar;
} 

private Type getType(string type) {
	Type tipoD = Type.Entero;
	switch(type) {
		case "booleano": tipoD = Type.Booleano; break;
		case "decimal": tipoD = Type.Decimal; break;
		case "cadena": tipoD = Type.Cadena; break;
		case "rutina": tipoD = Type.Rutina; break;
	}

	return tipoD;
}

void addVariable(string name, string tipo, bool isArr, int size) {
	if(!currentSymbolTable.ExistsInScope(name)) {
		Type tipoD = getType(tipo);

		Symbol symbol = new Variable {
			Name = name,
			IsArray = isArr,
			ArrayLength = size,
			Type = tipoD
		};
		
		currentSymbolTable.Add(symbol);
	}
	else
		SemErr($"El nombre {name} ya ha sido declarado en este scope.");
}

void checkVariableExists(string name) {
	var search = currentSymbolTable.Search(name);
	if (search == null)
		SemErr($"La variable {name} no ha sido declarada.");
	else if (!(search is Variable))
		SemErr($"El nombre {name} no se refiere a una variable.");
}

void checkFunctionExists(string name) {
	var search = currentSymbolTable.Search(name);
	if (search == null)
		SemErr($"La función {name} no ha sido declarada.");
	else if (!(search is Function))
		SemErr($"El nombre {name} no se refiere a una funcion.");
}

void checkIsArray(string name){
	var symbol = currentSymbolTable.Search(name) as Variable;
	if (!symbol.IsArray)
		SemErr($"La variable {name} no es un arreglo.");
}

void createNewSymbolTable(string name, List<Variable> parameters) {
	var newTable = new SymbolTable(currentSymbolTable, name);  
	currentSymbolTable.Children.Add(newTable);
	currentSymbolTable = newTable;
	addParameters(parameters.ToArray());
}

void addParameters(Variable[] parameters){
	foreach (var variable in parameters) {
		currentSymbolTable.Add(variable);
	}
}

void addFunction(string name, string tipo, List<Variable> parameters)
{
	if (!currentSymbolTable.ExistsInScope(name))
	{
		var fun = new Function {
			Name = name,
			Type = getType(tipo),
			Parameters = parameters
		};

		currentSymbolTable.Add(fun);
	}
	else {
		SemErr($"El nombre {name} ya ha sido declarado en este scope.");
	}
}

void checkParamAmount(string name, int amount)
{
	var fun = currentSymbolTable.Search(name) as Function;
	if (fun == null || fun.Parameters.Count != amount) {
		SemErr($"La funcion {name} no tiene {amount} parametros.");
	}
}



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
		currentSymbolTable = new SymbolTable(null, "global"); 
		Program();
	}

	void Program() {
		Vars();
		Funs();
		Main();
	}

	void Vars() {
		Expect(6);
		Expect(7);
		while (StartOf(1)) {
			Declaracion();
		}
	}

	void Funs() {
		Expect(8);
		Expect(7);
		while (StartOf(2)) {
			if (StartOf(1)) {
				Tipo();
			} else {
				Get();
			}
			var funType = t.val; 
			Expect(1);
			var funName = t.val; 
			Expect(5);
			var vars = new List<Variable>(); string tipo; 
			if (StartOf(1)) {
				Tipo();
				tipo = t.val; 
				Expect(1);
				vars.Add(new Variable(){Name=t.val, IsArray=false, ArrayLength=0, Type=getType(tipo)}); 
				while (la.kind == 10) {
					Get();
					Tipo();
					tipo = t.val; 
					Expect(1);
					vars.Add(new Variable(){Name=t.val, IsArray=false, ArrayLength=0, Type=getType(tipo)}); 
				}
			}
			Expect(11);
			addFunction(funName, funType, vars); 
			Bloque(funName, vars.ToArray());
		}
	}

	void Main() {
		Expect(12);
		Bloque("main", new Variable[]{});
	}

	void Declaracion() {
		string tipo; bool isArr = false; int size = 0; 
		Tipo();
		tipo = t.val; 
		if (la.kind == 18) {
			TipoArr(out size);
			isArr = true; 
		}
		Expect(1);
		addVariable(t.val, tipo, isArr, size); 
		while (la.kind == 10) {
			Get();
			Expect(1);
			addVariable(t.val, tipo, isArr, size); 
		}
		Expect(13);
	}

	void Tipo() {
		if (la.kind == 14) {
			Get();
		} else if (la.kind == 15) {
			Get();
		} else if (la.kind == 16) {
			Get();
		} else if (la.kind == 17) {
			Get();
		} else SynErr(45);
	}

	void Bloque(string name, Variable[] parameters) {
		Expect(20);
		createNewSymbolTable(name, new List<Variable>(parameters)); 
		while (StartOf(3)) {
			if (StartOf(1)) {
				Declaracion();
			} else if (la.kind == 23) {
				Condicion();
			} else if (la.kind == 25) {
				Ciclo();
			} else if (la.kind == 26) {
				Impresion();
			} else if (FollowedByLPar()) {
				Funcion();
				Expect(13);
			} else {
				Asignacion();
			}
		}
		Expect(21);
		currentSymbolTable = currentSymbolTable.Parent; 
	}

	void TipoArr(out int length) {
		Expect(18);
		Expect(3);
		length = int.Parse(t.val); 
		Expect(19);
	}

	void Condicion() {
		Expect(23);
		Expect(5);
		Expresion();
		Expect(11);
		Bloque("if", new Variable[]{});
		if (la.kind == 24) {
			Get();
			Bloque("else", new Variable[]{});
		}
	}

	void Ciclo() {
		Expect(25);
		Expect(5);
		Expresion();
		Expect(11);
		Bloque("while", new Variable[]{});
	}

	void Impresion() {
		Expect(26);
		Expect(5);
		Expresion();
		while (la.kind == 10) {
			Get();
			Expresion();
		}
		Expect(11);
		Expect(13);
	}

	void Funcion() {
		Expect(1);
		string name = t.val; checkFunctionExists(name); 
		Expect(5);
		var parameters = new List<object>(); 
		if (StartOf(4)) {
			Expresion();
			parameters.Add(""); 
			while (la.kind == 10) {
				Get();
				Expresion();
				parameters.Add(""); 
			}
		}
		Expect(11);
		checkParamAmount(name, parameters.Count); 
	}

	void Asignacion() {
		Variable();
		Expect(22);
		Expresion();
		Expect(13);
	}

	void Variable() {
		Expect(1);
		string name = t.val; checkVariableExists(name); 
		if (la.kind == 18) {
			Get();
			checkIsArray(name); 
			Expresion();
			Expect(19);
		}
	}

	void Expresion() {
		Exp();
		while (la.kind == 27 || la.kind == 28) {
			if (la.kind == 27) {
				Get();
			} else {
				Get();
			}
			Exp();
		}
	}

	void Exp() {
		Expt();
		if (StartOf(5)) {
			switch (la.kind) {
			case 29: {
				Get();
				break;
			}
			case 30: {
				Get();
				break;
			}
			case 31: {
				Get();
				break;
			}
			case 32: {
				Get();
				break;
			}
			case 33: {
				Get();
				break;
			}
			case 34: {
				Get();
				break;
			}
			}
			Expt();
		}
	}

	void Expt() {
		Termino();
		while (la.kind == 35 || la.kind == 36) {
			if (la.kind == 35) {
				Get();
			} else {
				Get();
			}
			Termino();
		}
	}

	void Termino() {
		Factor();
		while (la.kind == 37 || la.kind == 38 || la.kind == 39) {
			if (la.kind == 37) {
				Get();
			} else if (la.kind == 38) {
				Get();
			} else {
				Get();
			}
			Factor();
		}
	}

	void Factor() {
		if (la.kind == 5) {
			Get();
			Expresion();
			Expect(11);
		} else if (StartOf(6)) {
			if (la.kind == 35 || la.kind == 36) {
				if (la.kind == 35) {
					Get();
				} else {
					Get();
				}
			}
			Constante();
		} else SynErr(46);
	}

	void Constante() {
		if (la.kind == 3) {
			Get();
		} else if (la.kind == 4) {
			Get();
		} else if (la.kind == 42 || la.kind == 43) {
			Ctebol();
		} else if (la.kind == 2) {
			Get();
		} else if (la.kind == 40) {
			Aleatorio();
		} else if (la.kind == 41) {
			Lectura();
		} else if (FollowedByLPar()) {
			Funcion();
		} else if (la.kind == 1) {
			Variable();
		} else SynErr(47);
	}

	void Ctebol() {
		if (la.kind == 42) {
			Get();
		} else if (la.kind == 43) {
			Get();
		} else SynErr(48);
	}

	void Aleatorio() {
		Expect(40);
		Expect(5);
		Expect(11);
	}

	void Lectura() {
		Expect(41);
		Expect(5);
		Expect(11);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		ESpp();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_T, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_T, _x,_x}

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
			case 6: s = "\"variables\" expected"; break;
			case 7: s = "\":\" expected"; break;
			case 8: s = "\"funciones\" expected"; break;
			case 9: s = "\"rutina\" expected"; break;
			case 10: s = "\",\" expected"; break;
			case 11: s = "\")\" expected"; break;
			case 12: s = "\"main\" expected"; break;
			case 13: s = "\";\" expected"; break;
			case 14: s = "\"entero\" expected"; break;
			case 15: s = "\"decimal\" expected"; break;
			case 16: s = "\"booleano\" expected"; break;
			case 17: s = "\"cadena\" expected"; break;
			case 18: s = "\"[\" expected"; break;
			case 19: s = "\"]\" expected"; break;
			case 20: s = "\"{\" expected"; break;
			case 21: s = "\"}\" expected"; break;
			case 22: s = "\"=\" expected"; break;
			case 23: s = "\"si\" expected"; break;
			case 24: s = "\"sino\" expected"; break;
			case 25: s = "\"mientras\" expected"; break;
			case 26: s = "\"imprimir\" expected"; break;
			case 27: s = "\"&&\" expected"; break;
			case 28: s = "\"||\" expected"; break;
			case 29: s = "\">\" expected"; break;
			case 30: s = "\"<\" expected"; break;
			case 31: s = "\">=\" expected"; break;
			case 32: s = "\"<=\" expected"; break;
			case 33: s = "\"<>\" expected"; break;
			case 34: s = "\"==\" expected"; break;
			case 35: s = "\"+\" expected"; break;
			case 36: s = "\"-\" expected"; break;
			case 37: s = "\"*\" expected"; break;
			case 38: s = "\"/\" expected"; break;
			case 39: s = "\"%\" expected"; break;
			case 40: s = "\"aleatorio\" expected"; break;
			case 41: s = "\"lectura\" expected"; break;
			case 42: s = "\"verdadero\" expected"; break;
			case 43: s = "\"falso\" expected"; break;
			case 44: s = "??? expected"; break;
			case 45: s = "invalid Tipo"; break;
			case 46: s = "invalid Factor"; break;
			case 47: s = "invalid Constante"; break;
			case 48: s = "invalid Ctebol"; break;

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