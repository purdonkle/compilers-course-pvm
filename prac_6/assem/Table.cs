// Handle label table for simple PVM assembler
// P.D. Terry, Rhodes University, 2015

using Library;
using System.Collections.Generic;

namespace Assem {

  class LabelEntry {

    public string name;
    public Label label;

    public LabelEntry(string name, Label label) {
      this.name  = name;
      this.label = label;
    }

  } // end LabelEntry

// -------------------------------------------------------------------------------------

  class LabelTable {

    private static List<LabelEntry> list = new List<LabelEntry>();

    public static void Insert(LabelEntry entry) {
    // Inserts entry into label table
      list.Add(entry);
    } // insert

    public static LabelEntry Find(string name) {
    // Searches table for label entry matching name.  If found then returns entry.
    // If not found, returns null
      int i = 0;
      while (i < list.Count && !name.Equals(list[i].name)) i++;
      if (i >= list.Count) return null; else return list[i];
    } // find

    public static void CheckLabels() {
    // Checks that all labels have been defined (no forward references outstanding)
      for (int i = 0; i < list.Count; i++) {
        if (!list[i].label.IsDefined())
          Parser.SemError("undefined label - " + list[i].name);
      }
    } // CheckLabels

    public static void ListReferences(OutFile output) {
    // Cross reference list of all labels used on output file

    } // ListReferences

  } // end LabelTable

// -------------------------------------------------------------------------------------

  class VariableEntry {

    public string name;
    public int offset;

    public VariableEntry(string name, int offset) {
      this.name   = name;
      this.offset = offset;
    }

  } // end VariableEntry

// -------------------------------------------------------------------------------------

  class VarTable {

    private static List<VariableEntry> list = new List<VariableEntry>();
    private static int varOffset = 0;

    public static int FindOffset(string name) {
    // Searches table for variable entry matching name.  If found then returns the known offset.
    // If not found, makes an entry and updates the master offset
      return 0;    // dummy for initial testing
    } // FindOffset

    public static void ListReferences(OutFile output) {
    // Cross reference list of all variables on output file

    } // ListReferences

  } // end VarTable

} // end namespace
