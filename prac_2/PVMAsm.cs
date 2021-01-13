// Very simple assembler for simple stack machine
// P.D. Terry, Rhodes University, 2017
// This version for Practical 2


using Library;
using System;
using System.Text;

namespace Assem {

  class PVMAsm {

    static InFile src;
    static char ch;
    static bool okay;
    static int codeLen, stkBase;

    public static int CodeLength() {
      return codeLen;
    }

    public static int StackBase() {
      return stkBase;
    }

    static void SkipLabel() {
      do {
        ch = src.ReadChar();
        if (ch == ';') src.ReadLn(); // ignore comments
      } while (!(src.EOF() || char.IsLetter(ch)));
    }

    static void Error(string message, int codeLen) {
      Console.WriteLine(message + " at " + codeLen); okay = false;
    }

    static string ReadMnemonic() {
      StringBuilder sb = new StringBuilder();
      while (ch > ' ') {
        sb.Append(char.ToUpper(ch));
        ch = src.ReadChar();
      }
      string s = sb.ToString();
      // check for directives
      if (s == "ASSEM" || s == "BEGIN" || s == "END." ) return null;
      return s;
    }

    public static bool Assemble(string sourceName) {
    // Assembles source code from an input file sourceName and loads codeLen
    // words of code directly into memory PVM.mem[0 .. codeLen-1],
    // storing strings in the string pool at the top of memory in
    // PVM.mem[stkBase .. memSize-1].
    //
    // Returns true if assembly succeeded
    //
    // Instruction format :
    //    Instruction  = [ Label ] Opcode [ AddressField ] [ Comment ]
    //    Label        = [ "{" ] Integer [ "}" ]
    //    Opcode       = Mnemonic
    //    AddressField = Integer | "String"
    //    Comment      = String
    //
    // A string AddressField may only be used with a PRNS opcode
    // Instructions are supplied one to a line; terminated at end of input file

      string mnemonic;   // mnemonic for matching
      char quote;        // string delimiter

      src = new InFile(sourceName);
      if (src.OpenError()) {
        Console.WriteLine("Could not open input file\n");
        System.Environment.Exit(1);
      }
      Console.WriteLine("Assembling code ... \n");
      codeLen = 0;
      stkBase = PVM.memSize - 1;
      okay = true;
      do {
        SkipLabel();                                 // ignore labels at start of line
        if (!src.EOF()) {                            // we must have a line
          mnemonic = ReadMnemonic();                 // unpack mnemonic
          if (mnemonic == null) continue;            // ignore directives
          int op = PVM.OpCode(mnemonic);             // look it up
          PVM.mem[codeLen] = op;                     // store in memory
          switch (op) {
            case PVM.prns:                           // requires a string address field
              do quote = src.ReadChar();             // search for opening quote character
              while (quote != '"' && quote != '\'' && quote != '\n');
              codeLen = (codeLen + 1) % PVM.memSize;
              PVM.mem[codeLen] = stkBase - 1;        // pointer to start of string
              if (quote == '\n')
                Error("Missing string address", codeLen);
              else {                                 // stack string in literal pool
                ch = src.ReadChar();
                while (ch != quote && ch != '\n') {
                  if (ch == '\\') {
                    ch = src.ReadChar();
                    if (ch == '\n')                  // closing quote missing
                      Error("Malformed string", codeLen);
                    switch(ch) {
                      case 't'  : ch = '\t'; break;
                      case 'n'  : ch = '\n'; break;
                      case '\"' : ch = '\"'; break;
                      case '\'' : ch = '\''; break;
                      default: break;
                    }
                  }
                  stkBase--;
                  PVM.mem[stkBase] = ch;
                  ch = src.ReadChar();
                }
                if (ch == '\n')                      // closing quote missing
                  Error("Malformed string", codeLen);
              }
              stkBase--;
              PVM.mem[stkBase] = 0;                  // terminate string with nul
              break;
            case PVM.brn:                            // all require numeric address field
            case PVM.bze:
            case PVM.dsp:
            case PVM.lda:
            case PVM.ldc:
              codeLen = (codeLen + 1) % PVM.memSize;
              if (ch == '\n')                        // no field could be found
                Error("Missing address", codeLen);
              else {                                 // unpack it and store
                PVM.mem[codeLen] = src.ReadInt();
                if (src.Error()) Error("Bad address", codeLen);
              }
              break;
            case PVM.nul:                            // unrecognized mnemonic
              Console.Write(mnemonic);
              Error(" - Invalid opcode", codeLen);
              break;
            default:                                 // no address needed
              break;
          }
          if (!src.EOL()) src.ReadLn();              // skip comments
          codeLen = (codeLen + 1) % PVM.memSize;
        }
      } while (!src.EOF());
      for (int i = codeLen; i < stkBase; i++)        // fill with invalid OpCodes
        PVM.mem[i] = PVM.nul;
      return okay;
    }

  } // end PVMAsm

} // end namespace
