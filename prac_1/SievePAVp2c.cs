using Library;

class .\SievePAV {
  
  static public void Main(string[] args) {
    const int Max = 32000;
    bool[] uncrossed = new bool[Max];
    int i, n, k, it, iterations, primes = 0;
    { IO.Write("How many iterations? "); iterations = IO.ReadInt(); }
    bool display = iterations == 1;
    { IO.Write("Supply largest number to be tested "); n = IO.ReadInt(); }
    if (n > Max) {
      { IO.Write("n too large, sorry"); }
      return;
    }
    { IO.Write("Prime numbers between 2 and "); IO.Write(n); IO.Write("\n"); }
    { IO.Write("-----------------------------------\n"); }
    it = 1;
    while (it <= iterations) {
      primes = 0;
      i = 2;
      while (i <= n) {
        uncrossed[i - 2] = true;
        i = i + 1;
      }
      i = 2;
      while (i <= n) {
        if (uncrossed[i - 2]) {
          int mod = primes - ((primes / 8) * 8);
          if (display && (mod == 0))
            { IO.Write("\n"); }
          primes = primes + 1;
          if (display)
            { IO.Write(i); IO.Write("\t"); }
          k = i;
          uncrossed[k - 2] = false;
          k = k + i;
          while (k <= n) {
            uncrossed[k - 2] = false;
            k = k + i;
          }
        }
        i = i + 1;
      }
      it = it + 1;
      if (display)
        { IO.Write("\n"); }
    }
    { IO.Write("primes"); IO.Write(primes); }
  } // main
  
} // .\SievePAV