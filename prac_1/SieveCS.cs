// Sieve of Eratosthenes for finding primes 2 <= n <= Max (C# version)
// P.D. Terry,  Rhodes University, 2017

using Library;

class Sieve {

  public static void Main (string [] args) {
    const int Max = 32000;
    bool [] uncrossed = new bool [Max];        // the sieve
    int i, n, k, it, iterations, primes = 0;   // counters
    IO.Write("How many iterations? ");
    iterations = IO.ReadInt();
    bool display = iterations == 1;
    IO.Write("Supply largest number to be tested ");
    n = IO.ReadInt();
    if (n > Max) {
      IO.Write("n too large, sorry");
      System.Environment.Exit(1);
    }
    IO.WriteLine("Prime numbers between 2 and " + n);
    IO.WriteLine("-----------------------------------");

    IntSet mySet = new IntSet();
    mySet.Incl(5);
    IO.WriteLine("dsfsdfsdf" + mySet.ToString());

    for (it = 1; it <= iterations; it++) {
      primes = 0;
      for (i = 2; i <= n; i++)                 // clear sieve
        uncrossed[i] = true;
      for (i = 2; i <= n; i++)                 // the passes over the sieve
        if (uncrossed[i]) {
          if (display && primes % 8 == 0) {               // ensure line not too long
            IO.WriteLine();
          }
          primes++;
          if (display) IO.Write(i, 6);
          k = i;                               // now cross out multiples of i
          do {
            uncrossed[k] = false;
            k += i;
          } while (k <= n);
        }
      if (display) IO.WriteLine();
    }
    IO.Write(primes + " primes");
  } // main

} // Sieve
