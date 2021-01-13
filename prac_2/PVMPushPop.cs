// Definition of simple stack machine and simple emulator for Assem level 1
// Uses auxiliary methods Push, Pop and Next
// P.D. Terry, Rhodes University, 2017
// This version for Practical 2

// Edited by: Adam Purdon
// g18p8803

using Library;
using System;
using System.IO;
using System.Diagnostics;

namespace Assem {

  class Processor {
    public int sp;            // Stack pointer
    public int hp;            // Heap pointer
    public int gp;            // Global frame pointer
    public int fp;            // Local frame pointer
    public int mp;            // Mark stack pointer
    public int ir;            // Instruction register
    public int pc;            // Program counter
  } // end Processor

  class PVM {

  // Machine opcodes

    public const int
      nop    =    1,
      dsp    =    2,
      ldc    =    3,
      lda    =    4,
      ldv    =    5,
      sto    =    6,
      ldxa   =    7,
      inpi   =    8,
      prni   =    9,
      inpb   =   10,
      prnb   =   11,
      prns   =   12,
      prnl   =   13,
      neg    =   14,
      add    =   15,
      sub    =   16,
      mul    =   17,
      div    =   18,
      rem    =   19,
      not    =   20,
      and    =   21,
      or     =   22,
      ceq    =   23,
      cne    =   24,
      clt    =   25,
      cle    =   26,
      cgt    =   27,
      cge    =   28,
      brn    =   29,
      bze    =   30,
      anew   =   31,
      halt   =   32,
      stk    =   33,
      heap   =   34,
      ldl    =   35,
      stl    =   36,
      lda_0  =   37,
      lda_1  =   38,
      lda_2  =   39,
      lda_3  =   40,
      ldl_0  =   41,
      ldl_1  =   42,
      ldl_2  =   43,
      ldl_3  =   44,
      stl_0  =   45,
      stl_1  =   46,
      stl_2  =   47,
      stl_3  =   48,
      ldc_0  =   49,
      ldc_1  =   50,
      ldc_2  =   51,
      ldc_3  =   52,
      inpc   =   53,
      prnc   =   54,
      stoc   =   55,
      stlc   =   56,
      low    =   57,
      cap    =   58,
      islet  =   59,
      inc    =   60,
      dec    =   61,

      nul    =   99;               // leave gap for future

/* sorted 60 - 1

    public const int
      nop    =   60,
      dsp    =   59,
      ldc    =   58,
      lda    =   57,
      ldv    =   56,
      sto    =   55,
      ldxa   =   54,
      inpi   =   53,
      prni   =   52,
      inpb   =   51,
      prnb   =   50,
      prns   =   49,
      prnl   =   48,
      neg    =   47,
      add    =   46,
      sub    =   45,
      mul    =   44,
      div    =   43,
      rem    =   42,
      not    =   41,
      and    =   40,
      or     =   39,
      ceq    =   38,
      cne    =   37,
      clt    =   36,
      cle    =   35,
      cgt    =   34,
      cge    =   33,
      brn    =   32,
      bze    =   31,
      anew   =   30,
      halt   =   29,
      stk    =   28,
      heap   =   27,
      ldl    =   26,
      stl    =   25,
      lda_0  =   24,
      lda_1  =   23,
      lda_2  =   22,
      lda_3  =   21,
      ldl_0  =   20,
      ldl_1  =   19,
      ldl_2  =   18,
      ldl_3  =   17,
      stl_0  =   16,
      stl_1  =   15,
      stl_2  =   14,
      stl_3  =   13,
      ldc_0  =   12,
      ldc_1  =   11,
      ldc_2  =   10,
      ldc_3  =    9,
      inpc   =    8,
      prnc   =    7,
      stoc   =    6,
      stlc   =    5,
      low    =    4,
      islet  =    3,
      inc    =    2,
      dec    =    1,

      nul    =   99;               // leave gap for future

*/

/* sorted alphabetically

    public const int
      add    =    1,
      and    =    2,
      anew   =    3,
      brn    =    4,
      bze    =    5,
      ceq    =    6,
      cge    =    7,
      cgt    =    8,
      cle    =    9,
      clt    =   10,
      cne    =   11,
      dec    =   12,
      div    =   13,
      dsp    =   14,
      halt   =   15,
      heap   =   16,
      inc    =   17,
      inpb   =   18,
      inpc   =   19,
      inpi   =   20,
      islet  =   21,
      lda    =   22,
      lda_0  =   23,
      lda_1  =   24,
      lda_2  =   25,
      lda_3  =   26,
      ldc    =   27,
      ldc_0  =   28,
      ldc_1  =   29,
      ldc_2  =   30,
      ldc_3  =   31,
      ldl    =   32,
      ldl_0  =   33,
      ldl_1  =   34,
      ldl_2  =   35,
      ldl_3  =   36,
      ldv    =   37,
      ldxa   =   38,
      low    =   39,
      mul    =   40,
      neg    =   41,
      nop    =   42,
      not    =   43,
      or     =   44,
      prnb   =   45,
      prnc   =   46,
      prni   =   47,
      prnl   =   48,
      prns   =   49,
      rem    =   50,
      stk    =   51,
      stl    =   52,
      stl_0  =   53,
      stl_1  =   54,
      stl_2  =   55,
      stl_3  =   56,
      stlc   =   57,
      sto    =   58,
      stoc   =   59,
      sub    =   60,

      nul    =   99;               // leave gap for future

*/

    public static string[] mnemonics = new string[PVM.nul + 1];

  // Memory

    public const int memSize = 5120;         // Limit on memory
    public const int headerSize = 4;
    public static int[] mem;                 // Simulated memory
    static int stackBase, heapBase;          // Limits on cpu.sp

  // Program status

    const int                                                                   // CHANGES MADE HERE
      running  =  0,
      finished =  1,
      badMem   =  2,
      badData  =  3,
      noData   =  4,
      divZero  =  5,
      badOp    =  6,
      badInd   =  7,
      badVal   =  8,
      badAdr   =  9,
      badAll   = 10,
      nullRef  = 11;

    static int ps;

  // The processor

    static Processor cpu = new Processor();

  // The timer

    static Stopwatch timer = new Stopwatch();

  // Utilities

    static string padding = "                                                               ";
    const int maxInt = System.Int32.MaxValue;
    const int minInt = System.Int32.MinValue;
    const int maxChar = 255;

    static void StackDump(OutFile results, int pcNow) {
    // Dump local variable and stack area - useful for debugging
      int onLine = 0;
      results.Write("\nStack dump at " + pcNow);
      results.Write(" FP:"); results.Write(cpu.fp, 4);
      results.Write(" SP:"); results.WriteLine(cpu.sp, 4);
      for (int i = stackBase - 1; i >= cpu.sp; i--) {
        results.Write(i, 7); results.Write(mem[i], 5);
        onLine++; if (onLine % 8 == 0) results.WriteLine();
      }
      results.WriteLine();
    } // PVM.StackDump

    static void HeapDump(OutFile results, int pcNow) {
    // Dump heap area - useful for debugging
      if (heapBase == cpu.hp)
        results.WriteLine("Empty Heap");
      else {
        int onLine = 0;
        results.Write("\nHeap dump at " + pcNow);
        results.Write(" HP:"); results.Write(cpu.hp, 4);
        results.Write(" HB:"); results.WriteLine(heapBase, 4);
        for (int i = heapBase; i < cpu.hp; i++) {
          results.Write(i, 7); results.Write(mem[i], 5);
          onLine++; if (onLine % 8 == 0) results.WriteLine();
        }
        results.WriteLine();
      }
    } // PVM.HeapDump

    static void Trace(OutFile results, int pcNow, bool traceStack, bool traceHeap) {
    // Simple trace facility for run time debugging
      if (traceStack) StackDump(results, pcNow);
      if (traceHeap)  HeapDump(results, pcNow);
      results.Write(" PC:"); results.Write(pcNow, 5);
      results.Write(" FP:"); results.Write(cpu.fp, 5);
      results.Write(" SP:"); results.Write(cpu.sp, 5);
      results.Write(" HP:"); results.Write(cpu.hp, 5);
      results.Write(" TOS:");
      if (cpu.sp < memSize)
        results.Write(mem[cpu.sp], 5);
      else
        results.Write(" ????");
      results.Write("  " + mnemonics[cpu.ir], -8);
      switch (cpu.ir) {
        case PVM.brn:
        case PVM.bze:
        case PVM.dsp:
        case PVM.lda:
        case PVM.ldc:
        case PVM.prns:
          results.Write(mem[cpu.pc], 7); break;
        default: break;
      }
      results.WriteLine();
    } // PVM.Trace

    static void PostMortem(OutFile results, int pcNow) {
    // Reports run time error and position
      results.WriteLine();
      switch (ps) {
        case badMem:  results.Write("Memory violation"); break;
        case badData: results.Write("Invalid data"); break;
        case noData:  results.Write("No more data"); break;
        case divZero: results.Write("Division by zero"); break;
        case badOp:   results.Write("Illegal opcode"); break;
        case badInd:  results.Write("Subscript out of range"); break;
        case badVal:  results.Write("Value out of range"); break;
        case badAdr:  results.Write("Bad address"); break;
        case badAll:  results.Write("Heap allocation error"); break;
        case nullRef: results.Write("Null reference"); break;
        default:      results.Write("Interpreter error!"); break;
      }
      results.WriteLine(" at " + pcNow);
    } // PVM.PostMortem

  // The interpreters and utility methods

    static int Next() {
    // Fetches next word of program and bumps program counter
      return mem[cpu.pc++];
    } // PVM.Next

    static void Push(int value) {
    // Bumps stack pointer and pushes value onto stack
      mem[--cpu.sp] = value;
      if (cpu.sp < cpu.hp) ps = badMem;
    } // PVM.Push

    static int Pop() {
    // Pops and returns top value on stack and bumps stack pointer
      if (cpu.sp == cpu.fp) ps = badMem;
      return mem[cpu.sp++];
    } // PVM.Pop

    static bool InBounds(int p) {
    // Check that memory pointer p does not go out of bounds.  This should not
    // happen with correct code, but it is just as well to check
      if (p < heapBase || p > memSize) ps = badMem;
      return (ps == running);
    } // PVM.InBounds

    public static void Emulator(int initPC, int codeLen, int initSP,
                                InFile data, OutFile results, bool tracing,
                                bool traceStack, bool traceHeap) {
    // Emulates action of the codeLen instructions stored in mem[0 .. codeLen-1], with
    // program counter initialized to initPC, stack pointer initialized to initSP.
    // data and results are used for I/O.  Tracing at the code level may be requested

      int pcNow;                  // current program counter
      int loop;                   // internal loops
      int tos, sos;               // value popped from stack
      int adr;                    // effective address for memory accesses
      stackBase = initSP;
      heapBase = codeLen;         // initialize boundaries
      cpu.hp = heapBase;          // initialize registers
      cpu.sp = stackBase;
      cpu.gp = stackBase;
      cpu.mp = stackBase;
      cpu.fp = stackBase;
      cpu.pc = initPC;            // initialize program counter
      ps = running;               // prepare to execute
      int ops = 0;
      timer.Start();
      do {
        ops++;
        pcNow = cpu.pc;           // retain for tracing/postmortem
        if (cpu.pc < 0 || cpu.pc >= codeLen) {
          ps = badAdr;
          break;
        }
        cpu.ir = Next();          // fetch
        if (tracing) Trace(results, pcNow, traceStack, traceHeap);
        switch (cpu.ir) {         // execute
          case PVM.nop:           // no operation
            break;
          case PVM.dsp:           // decrement stack pointer (allocate space for variables)
            int localSpace = Next();
            cpu.sp -= localSpace;
            if (InBounds(cpu.sp)) // initialize
              for (loop = 0; loop < localSpace; loop++)
                mem[cpu.sp + loop] = 0;
            break;
          case PVM.ldc:           // push constant value
            Push(Next());
            break;
          case PVM.lda:           // push local address
            adr = cpu.fp - 1 - Next();
            if (InBounds(adr)) Push(adr);
            break;
          case PVM.ldv:           // dereference
            Push(mem[Pop()]);
            break;
          case PVM.sto:           // store
            tos = Pop(); adr = Pop();
            if (InBounds(adr)) mem[adr] = tos;
            break;
          case PVM.ldxa:          // heap array indexing
            adr = Pop();
            int heapPtr = Pop();
            if (heapPtr == 0) ps = nullRef;
            else if (heapPtr < heapBase || heapPtr >= cpu.hp) ps = badMem;
            else if (adr < 0 || adr >= mem[heapPtr]) ps = badInd;
            else Push(heapPtr + adr + 1);
            break;
          case PVM.inpi:          // integer input
            adr = Pop();
            if (InBounds(adr)) {
              mem[adr] = data.ReadInt();
              if (data.Error()) ps = badData;
            }
            break;
          case PVM.prni:          // integer output
            if (tracing) results.Write(padding);
            results.Write(Pop(), 0);
            if (tracing) results.WriteLine();
            break;
          case PVM.inpb:          // boolean input
            adr = Pop();
            if (InBounds(adr)) {
              mem[adr] = data.ReadBool() ? 1 : 0;
              if (data.Error()) ps = badData;
            }
            break;
          case PVM.prnb:          // boolean output
            if (tracing) results.Write(padding);
            if (Pop() != 0) results.Write(" true  "); else results.Write(" false ");
            if (tracing) results.WriteLine();
            break;
          case PVM.prns:          // string output
            if (tracing) results.Write(padding);
            loop = Next();
            while (ps == running && mem[loop] != 0) {
              results.Write((char) mem[loop]); loop--;
              if (loop < stackBase) ps = badMem;
            }
            if (tracing) results.WriteLine();
            break;
          case PVM.prnl:          // newline
            results.WriteLine();
            break;
          case PVM.neg:           // integer negation
            Push(-Pop());
            break;
          case PVM.add:           // integer addition
            tos = Pop(); Push(Pop() + tos);
            break;
          case PVM.sub:           // integer subtraction
            tos = Pop(); Push(Pop() - tos);
            break;
          case PVM.mul:           // integer multiplication                         CHANGES MADE HERE: multipicative overflow and underflow                      
            tos = Pop();
            long overFlowCheck = (long)tos * (long)Pop();
            if (overFlowCheck > maxInt || overFlowCheck < minInt) {
                ps = badVal;
            }
            else {
                Push((int)overFlowCheck);
            }
            break;
          case PVM.div:           // integer division (quotient)                    CHANGES MADE HERE: divide by zero
            tos = Pop();
            if (tos == 0) {       // addition for divZero check
                ps = divZero;
            } else {
                Push(Pop() / tos);
            }
            break;
          case PVM.rem:           // integer division (remainder)
            tos = Pop(); Push(Pop() % tos);
            break;
          case PVM.not:           // logical negation
            Push(Pop() == 0 ? 1 : 0);
            break;
          case PVM.and:           // logical and
            tos = Pop(); Push(Pop() & tos);
            break;
          case PVM.or:            // logical or
            tos = Pop(); Push(Pop() | tos);
            break;
          case PVM.ceq:           // logical equality
            tos = Pop(); Push(Pop() == tos ? 1 : 0);
            break;
          case PVM.cne:           // logical inequality
            tos = Pop(); Push(Pop() != tos ? 1 : 0);
            break;
          case PVM.clt:           // logical less
            tos = Pop(); Push(Pop() <  tos ? 1 : 0);
            break;
          case PVM.cle:           // logical less or equal
            tos = Pop(); Push(Pop() <= tos ? 1 : 0);
            break;
          case PVM.cgt:           // logical greater
            tos = Pop(); Push(Pop() >  tos ? 1 : 0);
            break;
          case PVM.cge:           // logical greater or equal
            tos = Pop(); Push(Pop() >= tos ? 1 : 0);
            break;
          case PVM.brn:           // unconditional branch
            cpu.pc = Next();
            if (cpu.pc < 0 || cpu.pc >= codeLen) ps = badAdr;
            break;
          case PVM.bze:           // pop top of stack, branch if false
            int target = Next();
            if (Pop() == 0) {
              cpu.pc = target;
              if (cpu.pc < 0 || cpu.pc >= codeLen) ps = badAdr;
            }
            break;
          case PVM.anew:          // heap array allocation
            int size = Pop();
            if (size <= 0 || size + 1 > cpu.sp - cpu.hp - 2)
              ps = badAll;
            else {
              mem[cpu.hp] = size;
              Push(cpu.hp);
              cpu.hp += size + 1;
            }
            break;
          case PVM.halt:          // halt
            ps = finished;
            break;
          case PVM.stk:           // stack dump (debugging)
            StackDump(results, pcNow);
            break;
          case PVM.heap:           // heap dump (debugging)
            HeapDump(results, pcNow);
            break;
          case PVM.ldc_0:         // push constant 0
          case PVM.ldc_1:         // push constant 1
          case PVM.ldc_2:         // push constant 2
          case PVM.ldc_3:         // push constant 3
          case PVM.lda_0:         // push local address 0
          case PVM.lda_1:         // push local address 1
          case PVM.lda_2:         // push local address 2
          case PVM.lda_3:         // push local address 3
          case PVM.ldl:           // push local value
          case PVM.ldl_0:         // push value of local variable 0
          case PVM.ldl_1:         // push value of local variable 1
          case PVM.ldl_2:         // push value of local variable 2
          case PVM.ldl_3:         // push value of local variable 3
          case PVM.stl:           // store local value
          case PVM.stlc:          // store local value
          case PVM.stl_0:         // pop to local variable 0
          case PVM.stl_1:         // pop to local variable 1
          case PVM.stl_2:         // pop to local variable 2
          case PVM.stl_3:         // pop to local variable 3
          case PVM.stoc:          // character checked store
          case PVM.inpc:          // character input
          case PVM.prnc:          // character output
          case PVM.cap:           // toUpperCase
          case PVM.low:           // toLowerCase
          case PVM.islet:         // isLetter
          case PVM.inc:           // ++
            Pop(); Push(Pop() + 1);                                                     // CHANGES MADE HERE: added increment
            break;
          case PVM.dec:           // --                                                 // CHANGES MADE HERE: added decrement
            Pop(); Push(Pop() - 1);
            break;
          default:                // unrecognized opcode
            ps = badOp;
            break;
        }
      } while (ps == running);
      TimeSpan ts = timer.Elapsed;

      // Format and display the TimeSpan value.
      string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                         ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
      Console.WriteLine("\n\n" + ops + " operations.  Run Time " + elapsedTime + "\n\n");
      if (ps != finished) PostMortem(results, pcNow);
      timer.Reset();
      timer.Stop();
    } // PVM.Emulator

    public static void QuickInterpret(int codeLen, int initSP) {
    // Interprets the codeLen instructions stored in mem, with stack pointer
    // initialized to initSP.  Use StdIn and StdOut without asking
      Console.WriteLine("\nHit <Enter> to start");
      Console.ReadLine();
      bool tracing = false;
      InFile data = new InFile("");
      OutFile results = new OutFile("");
      Emulator(0, codeLen, initSP, data, results, false, false, false);
    } // PVM.QuickInterpret

    public static void Interpret(int codeLen, int initSP) {
    // Interactively opens data and results files.  Then interprets the codeLen
    // instructions stored in mem, with stack pointer initialized to initSP
      Console.Write("\nTrace execution (y/N/q)? ");
      char reply = (Console.ReadLine() + " ").ToUpper()[0];
      bool traceStack = false, traceHeap = false;
      if (reply != 'Q') {
        bool tracing = reply == 'Y';
        if (tracing) {
          Console.Write("\nTrace Stack (y/N)? ");
          traceStack = (Console.ReadLine() + " ").ToUpper()[0] == 'Y';
          Console.Write("\nTrace Heap (y/N)? ");
          traceHeap = (Console.ReadLine() + " ").ToUpper()[0] == 'Y';
        }
        Console.Write("\nData file [STDIN] ? ");
        InFile data = new InFile(Console.ReadLine());
        Console.Write("\nResults file [STDOUT] ? ");
        OutFile results = new OutFile(Console.ReadLine());
        Emulator(0, codeLen, initSP, data, results, tracing, traceStack, traceHeap);
        results.Close();
        data.Close();
      }
    } // PVM.Interpret

    public static void ListCode(string fileName, int codeLen) {
    // Lists the codeLen instructions stored in mem on a named output file
      int i, j, n;

      if (fileName == null) return;
      OutFile codeFile = new OutFile(fileName);

      /* ------------- The following may be useful for debugging the interpreter
      i = 0;
      while (i < codeLen) {
        codeFile.Write(mem[i], 5);
        if ((i + 1) % 15 == 0) codeFile.WriteLine();
        i++;
      }
      codeFile.WriteLine();

      ------------- */

      i = 0;
      codeFile.WriteLine("ASSEM\nBEGIN");
      while (i < codeLen) {
        int o = mem[i] % (PVM.nul + 1); // force in range
        codeFile.Write("  {");
        codeFile.Write(i, 5);
        codeFile.Write(" } ");
        codeFile.Write(mnemonics[o], -8);
        switch (o) {
          case PVM.brn:
          case PVM.bze:
          case PVM.dsp:
          case PVM.lda:
          case PVM.ldc:
            i = (i + 1) % memSize; codeFile.Write(mem[i]);
            break;

          case PVM.prns:
            i = (i + 1) % memSize;
            j = mem[i]; codeFile.Write(" \"");
            while (mem[j] != 0) {
              switch (mem[j]) {
                case '\\' : codeFile.Write("\\\\"); break;
                case '\"' : codeFile.Write("\\\""); break;
                case '\'' : codeFile.Write("\\\'"); break;
                case '\b' : codeFile.Write("\\b"); break;
                case '\t' : codeFile.Write("\\t"); break;
                case '\n' : codeFile.Write("\\n"); break;
                case '\f' : codeFile.Write("\\f"); break;
                case '\r' : codeFile.Write("\\r"); break;
                default   : codeFile.Write((char) mem[j]); break;
              }
              j--;
            }
            codeFile.Write("\"");
            break;
        }
        i = (i + 1) % memSize;
        codeFile.WriteLine();
      }
      codeFile.WriteLine("END.");
      codeFile.Close();
    } // PVM.ListCode

    public static int OpCode(string str) {
    // Maps str to opcode, or to PVM.nul if no match can be found
      int op = 0;
      while (op != PVM.nul && !(str.ToUpper().Equals(mnemonics[op]))) op++;
      return op;
    } // PVM.Opcode

    public static void Init() {
    // Initializes stack machine
      mem = new int [memSize + 1];  // virtual machine memory
      for (int i = 0; i <= memSize; i++) mem[i] = 0;
      // Initialize mnemonic table this way for ease of modification in exercises
      for (int i = 0; i <= PVM.nul; i++) mnemonics[i] = "";
      mnemonics[PVM.add]    = "ADD";
      mnemonics[PVM.and]    = "AND";
      mnemonics[PVM.anew]   = "ANEW";
      mnemonics[PVM.brn]    = "BRN";
      mnemonics[PVM.bze]    = "BZE";
      mnemonics[PVM.cap]    = "CAP";
      mnemonics[PVM.ceq]    = "CEQ";
      mnemonics[PVM.cge]    = "CGE";
      mnemonics[PVM.cgt]    = "CGT";
      mnemonics[PVM.cle]    = "CLE";
      mnemonics[PVM.clt]    = "CLT";
      mnemonics[PVM.cne]    = "CNE";
      mnemonics[PVM.dec]    = "DEC";
      mnemonics[PVM.div]    = "DIV";
      mnemonics[PVM.dsp]    = "DSP";
      mnemonics[PVM.halt]   = "HALT";
      mnemonics[PVM.heap]   = "HEAP";
      mnemonics[PVM.inc]    = "INC";
      mnemonics[PVM.inpb]   = "INPB";
      mnemonics[PVM.inpc]   = "INPC";
      mnemonics[PVM.inpi]   = "INPI";
      mnemonics[PVM.islet]  = "ISLET";
      mnemonics[PVM.lda]    = "LDA";
      mnemonics[PVM.lda_0]  = "LDA_0";
      mnemonics[PVM.lda_1]  = "LDA_1";
      mnemonics[PVM.lda_2]  = "LDA_2";
      mnemonics[PVM.lda_3]  = "LDA_3";
      mnemonics[PVM.ldc]    = "LDC";
      mnemonics[PVM.ldc_0]  = "LDC_0";
      mnemonics[PVM.ldc_1]  = "LDC_1";
      mnemonics[PVM.ldc_2]  = "LDC_2";
      mnemonics[PVM.ldc_3]  = "LDC_3";
      mnemonics[PVM.ldl]    = "LDL";
      mnemonics[PVM.ldl_0]  = "LDL_0";
      mnemonics[PVM.ldl_1]  = "LDL_1";
      mnemonics[PVM.ldl_2]  = "LDL_2";
      mnemonics[PVM.ldl_3]  = "LDL_3";
      mnemonics[PVM.ldv]    = "LDV";
      mnemonics[PVM.ldxa]   = "LDXA";
      mnemonics[PVM.low]    = "LOW";
      mnemonics[PVM.mul]    = "MUL";
      mnemonics[PVM.neg]    = "NEG";
      mnemonics[PVM.nop]    = "NOP";
      mnemonics[PVM.not]    = "NOT";
      mnemonics[PVM.nul]    = "NUL";
      mnemonics[PVM.or]     = "OR";
      mnemonics[PVM.prnb]   = "PRNB";
      mnemonics[PVM.prnc]   = "PRNC";
      mnemonics[PVM.prni]   = "PRNI";
      mnemonics[PVM.prnl]   = "PRNL";
      mnemonics[PVM.prns]   = "PRNS";
      mnemonics[PVM.rem]    = "REM";
      mnemonics[PVM.stk]    = "STK";
      mnemonics[PVM.stl]    = "STL";
      mnemonics[PVM.stl_0]  = "STL_0";
      mnemonics[PVM.stl_1]  = "STL_1";
      mnemonics[PVM.stl_2]  = "STL_2";
      mnemonics[PVM.stl_3]  = "STL_3";
      mnemonics[PVM.stlc]   = "STLC";
      mnemonics[PVM.sto]    = "STO";
      mnemonics[PVM.stoc]   = "STOC";
      mnemonics[PVM.sub]    = "SUB";
    } // PVM.Init

  } // end PVM

} // end namespace
