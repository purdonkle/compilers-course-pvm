  // Adam Purdon
  // A scanner and parser intended for use in conjunction with c/cpp code

  // This is a skeleton program for developing a parser for C declarations
  // P.D. Terry, Rhodes University, 2015

  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    } // constructor

  } // Token

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int
      noSym        =  0,
      EOFSym       =  1,
      intSym       =  2,    // ADDED: other symbols for language
      charSym      =  3,
      voidSym      =  4,
      numSym       =  5,
      identSym     =  6,
      lparenSym    =  7,
      rparenSym    =  8,
      lbrackSym    =  9,
      rbrackSym    =  10,
      commaSym     =  11,
      semicolonSym =  12;
      
      // and others like this

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = ch == EOF;
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() {
    // Scans for next sym from input
      while (ch > EOF && ch <= ' ') GetChar();
      StringBuilder symLex = new StringBuilder();
      int symKind = noSym;

      if ((Char.IsLetter(ch) || ch == '_')) {               // ADDED: symbol is int/char/void/ident
        do {
          symLex.Append(ch); GetChar();
        } while (Char.IsLetterOrDigit(ch) || ch == '_');
        if (symLex.ToString() == "int") {
          symKind = intSym;
        } else if (symLex.ToString() == "char") {
          symKind = charSym;
        } else if (symLex.ToString() == "void") {
          symKind = voidSym;
        } else {
          symKind = identSym;
        }
      } 
      else if (Char.IsDigit(ch)) {                          // ADDED: symbol is num
        do {
          symLex.Append(ch); GetChar();
        } while (Char.IsDigit(ch));
        symKind = numSym;
      }
      else {                                                // ADDED: symbol is '(', ')', '[', ']', ',', ';', noSym
        symLex.Append(ch);
        switch (ch) {
          case EOF:
            symLex = new StringBuilder("EOF");
            symKind = EOFSym; break;
          case '(':
            symKind = lparenSym; GetChar(); 
            break;
          case ')':
            symKind = rparenSym; GetChar();
            break;
          case '[':
            symKind = lbrackSym; GetChar();
            break;
          case ']':
            symKind = rbrackSym; GetChar();
            break;
          case ',':
            symKind = commaSym; GetChar();
            break;
          case ';':
            symKind = semicolonSym; GetChar();
            break;
          default:
            symKind = noSym; GetChar();
            break;
        }
      }

      sym = new Token(symKind, symLex.ToString());
    } // GetSym

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
    // Checks that lookahead token is wantedSym
      if (sym.kind == wantedSym) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
    // Checks that lookahead token is in allowedSet
      if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept

    static void CDecls() {                                                                    
    // Cdecls = { DecList } EOF 
      while (sym.kind != EOFSym) DecList();
      Accept(EOFSym, "EOF expected");
    }

    static void DecList() {                                 // ADDED DecList
    // Type OneDecl { "," OneDecl } ";"
      Accept(new IntSet(intSym, voidSym, charSym), "int, char or void expected");
      OneDecl();
      while (sym.kind == commaSym) {
        Accept(commaSym, "',' expected");
        OneDecl();
      }
      Accept(semicolonSym, "';' expected");
    }

    static void OneDecl() {                                 // ADDED OneDecl
    // ident [ Suffix ]
      Accept(identSym, "identifier expected");
      if (sym.kind == lbrackSym || sym.kind == lparenSym) {
        Suffix();
      } 
    }

    static void Suffix() {                                  // ADDED Suffix
      if (sym.kind == lbrackSym) {
        Array();
        while (sym.kind == lbrackSym) {
          Array();
        }
      } else {
        Params();
      }
    }

    static void Array() {                                   // ADDED Array
      Accept(lbrackSym, "'[' expected");
      if (sym.kind == numSym) Accept(numSym, "number expected");
      Accept(rbrackSym, "']' expected");
    }

    static void Params() {                                  // Added Params
    // "(" [ OneParam { "," OneParam } ] ")"
      Accept(lparenSym, "'(' expected");
      if (sym.kind != rparenSym) {
        OneParam();
        while (sym.kind == commaSym) {
          Accept(commaSym, "',' expected");
          OneParam();
        }
      }
      Accept(rparenSym, "']' expected");
    }

    static void OneParam() {                                // Added OneParam
    // Type [ OneDecl ]
      Accept(new IntSet(intSym, voidSym, charSym), "int, char or void expected");
      if (sym.kind == identSym) {
        OneDecl();
      }
    }

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:
  /*
      do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);
  */

                                                            // EDITED 
      GetSym();                                   // Lookahead symbol
      CDecls();                                   // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");

      output.Close();
    } // Main

  } // Declarations

