// Code Generation for Parva assembler (C# version)
// P.D. Terry, Rhodes University, 2015

using System;
using System.IO;

namespace Assem {

  class Label {
    private int memAdr;      // address if this.defined, else last forward reference
    private bool defined;    // true once this.memAdr is known

    public Label(bool known) {
    // Constructor for label, possibly at already known location
      if (known) this.memAdr = CodeGen.GetCodeLength();
      else this.memAdr = CodeGen.undefined;  // mark end of forward reference chain
      this.defined = known;
    }

    public int Address() {
    // Returns memAdr if known, otherwise effectively adds to a forward reference
    // chain that will be resolved if and when Here() is called and returns the
    // address of the most recent forward reference
      int adr = memAdr;
      if (!defined) memAdr = CodeGen.GetCodeLength();
      return adr;
    }

    public void Here() {
    // Defines memAdr of this label to be at current location counter after fixing
    // any outstanding forward references
      if (defined) Parser.SemError("Compiler error - bad label");
      else CodeGen.BackPatch(memAdr);
      memAdr = CodeGen.GetCodeLength();
      defined = true;
    }

    public bool IsDefined() {
    // Returns true if the location of this label has been established
      return defined;
    }

    public override string ToString() {
      return memAdr.ToString();
    }

  } // end Label

  class CodeGen {
    static bool generatingCode = true;
    static int codeTop = 0, stkTop = PVM.memSize;

    public const int
      undefined  = -1;

    private static void Emit(int word) {
    // Code generator for single word
      if (!generatingCode) return;
      if (codeTop >= stkTop) {
        Parser.SemError("program too long"); generatingCode = false;
      }
      else {
        PVM.mem[codeTop] = word; codeTop++;
      }
    }

    public static void WriteString(string str) {
    // Generates code to output string stored at known location
      int l = str.Length, first = stkTop - 1;
      if (stkTop <= codeTop + l + 1) {
        Parser.SemError("program too long"); generatingCode = false;
        return;
      }
      for (int i = 0; i < l; i++) {
        stkTop--; PVM.mem[stkTop] = str[i];
      }
      stkTop--; PVM.mem[stkTop] = 0;
      Emit(PVM.prns); Emit(first);
    }

    public static void BackPatch(int adr) {
    // Stores the current location counter as the address field of the branch or call
    // instruction currently holding a forward reference to adr and repeatedly
    // works through a linked list of such instructions
      while (adr != undefined) {
        int nextAdr = PVM.mem[adr];
        PVM.mem[adr] = codeTop;
        adr = nextAdr;
      }
    }

    public static int GetCodeLength() {
    // Returns codeTop = length of generated code
      return codeTop;
    }

    public static int GetInitSP() {
    // Returns stkTop = position for initial stack pointer
      return stkTop;
    }

    public static void OneWord(string mnemonic) {
    // Inline assembly of one word instruction with no operand
      Emit(PVM.OpCode(mnemonic));
    }

    public static void TwoWord(string mnemonic, int adr) {
    // Inline assembly of two word instruction with integer operand
      Emit(PVM.OpCode(mnemonic)); Emit(adr);
    }

    public static void Branch(string mnemonic, Label adr) {
    // Inline assembly of two word branch style instruction with Label operand
      Emit(PVM.OpCode(mnemonic)); Emit(adr.Address());
    }

  } // end CodeGen

} // namespace
