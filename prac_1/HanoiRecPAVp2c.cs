using Library;

class HanoiRecPAV {
  
  static public int moves;
  
  static public bool display;
  
  static public void Hanoi(int n, int a, int b, int c) {
    if (n > 0) {
      Hanoi(n - 1, a, c, b);
      if (display)
        { IO.Write("Move disk "); IO.Write(n); IO.Write(" from "); IO.Write(a); IO.Write(" to "); IO.Write(b); IO.Write("\n"); }
      moves = moves + 1;
      Hanoi(n - 1, c, b, a);
    }
  } // Hanoi
  
  static public void Main(string[] args) {
    int disks;
    { IO.Write("How many disks? "); disks = IO.ReadInt(); }
    int iter;
    { IO.Write("How many iterations "); iter = IO.ReadInt(); }
    display = iter == 1;
    while (iter > 0) {
      moves = 0;
      Hanoi(disks, 1, 2, 3);
      { IO.Write(moves); IO.Write(" moves\n"); }
      iter = iter - 1;
    }
  } // Main
  
} // HanoiRecPAV