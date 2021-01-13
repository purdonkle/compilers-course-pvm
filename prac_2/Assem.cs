// Assembler/Interpreter for simple stack machine
// P.D. Terry, Rhodes University, 2017
// This version for Practical 2

using System;
using System.IO;
using System.Text;

namespace Assem {

  class Assem {

    static string newFileName(string s, string ext) {
    // Returns a new string for a file name with the extension in s replaced by ext
      int i = s.LastIndexOf('.');
      if (i < 0) return s + ext; else return s.Substring(0, i) + ext;
    }

    public static void Main(string[] args) {
      if (args.Length == 0) {  // check on correct parameter usage
        Console.WriteLine("Usage: ASSEM source [ immediate ]");
      }
      else {
        string sourceName = args[0];
        bool immediate = args.Length > 1;
        string codeName = newFileName(sourceName, ".cod");
        PVM.Init();

        bool assembledOK = PVMAsm.Assemble(sourceName);
        int codeLength = PVMAsm.CodeLength();
        int initSP = PVMAsm.StackBase();
        PVM.ListCode(codeName, codeLength);
        if (!assembledOK || codeLength == 0)
          Console.WriteLine("Unable to interpret code");
        else {
          if (immediate) PVM.QuickInterpret(codeLength, initSP);
          char reply = 'n';
          do {
            Console.Write("\n\nInterpret [y/N]? ");
            reply = (Console.ReadLine() + " ").ToUpper()[0];
            if (reply == 'Y') PVM.Interpret(codeLength, initSP);
          } while (reply == 'Y');
        }
      }
    }

  } // end Assem

} // end namespace
