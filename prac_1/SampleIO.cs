// Program to demonstrate Infile, OutFile and IntSet classes
// P.D. Terry, Rhodes University, 2017

using Library;
using System;

class SampleIO {

  public static void Main(string[] args) {
  //                                        check that arguments have been supplied
    if (args.Length != 2) {
      Console.WriteLine("missing args");
      System.Environment.Exit(1);
    }
  //                                        attempt to open data file
    InFile data = new InFile(args[0]);
    if (data.OpenError()) {
      Console.WriteLine("cannot open " + args[0]);
      System.Environment.Exit(1);
    }
  //                                        attempt to open results file
    OutFile results = new OutFile(args[1]);
    if (results.OpenError()) {
      Console.WriteLine("cannot open " + args[1]);
      System.Environment.Exit(1);
    }
  //                                        various initializations
    int total = 0;
    IntSet mySet = new IntSet();
    IntSet smallSet = new IntSet(1, 2, 3, 4, 5);
    string smallSetStr = smallSet.ToString();
  //                                        read and process data file
    int item = data.ReadInt();
    while (!data.NoMoreData()) {
      total = total + item;
      if (item > 0) mySet.Incl(item);
      item = data.ReadInt();
    }
  //                                        write various results to output file
    results.Write("total = ");
    results.WriteLine(total, 5);
    results.WriteLine("unique positive numbers " + mySet.ToString());
    results.WriteLine("union with " + smallSetStr
                       + " = " + mySet.Union(smallSet).ToString());
    results.WriteLine("intersection with " + smallSetStr
                       + " = " + mySet.Intersection(smallSet).ToString());

    /* or simply
      results.WriteLine("union with " + smallSetStr + " = " + mySet.Union(smallSet));
      results.WriteLine("intersection with " + smallSetStr + " = " + mySet.Intersection(smallSet));
    */

    results.Close();
  } // Main

} // SampleIO
