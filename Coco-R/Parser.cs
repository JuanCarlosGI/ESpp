
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

bool FollowedByLPar() {
 return scanner.Peek().kind == _lpar;
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
			Expect(1);
			Expect(5);
			if (StartOf(1)) {
				Tipo();
				Expect(1);
				while (la.kind == 10) {
					Get();
					Tipo();
					Expect(1);
				}
			}
			Expect(11);
			Bloque();
		}
	}

	void Main() {
		Expect(12);
		Bloque();
	}

	void Declaracion() {
		Tipo();
		if (la.kind == 18) {
			TipoArr();
		}
		Expect(1);
		while (la.kind == 10) {
			Get();
			Expect(1);
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

	void Bloque() {
		Expect(20);
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
	}

	void TipoArr() {
		Expect(18);
		Expect(3);
		Expect(19);
	}

	void Condicion() {
		Expect(23);
		Expect(5);
		Expresion();
		Expect(11);
		Bloque();
		if (la.kind == 24) {
			Get();
			Bloque();
		}
	}

	void Ciclo() {
		Expect(25);
		Expect(5);
		Expresion();
		Expect(11);
		Bloque();
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
		Expect(5);
		if (StartOf(4)) {
			Expresion();
			while (la.kind == 10) {
				Get();
				Expresion();
			}
		}
		Expect(11);
	}

	void Asignacion() {
		Variable();
		Expect(22);
		Expresion();
		Expect(13);
	}

	void Variable() {
		Expect(1);
		if (la.kind == 18) {
			Get();
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