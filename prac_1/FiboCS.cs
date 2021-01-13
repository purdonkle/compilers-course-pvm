// Print a table of Fibonacci numbers using (slow) recursive definition
// P.D. Terry,  Rhodes University, 2017

using Library;

class Fibo {

  static int fib(int m) {
  // Compute m-th term in Fibonacci series 0,1,1,2 ...
    if (m == 0) return 0;
    if (m == 1) return 1;
    return fib(m-1) + fib(m-2);
  } // fib

  public static void Main(string[] args) {
    int i, limit;
    IO.Write("Supply upper limit ");
    limit = IO.ReadInt();
    i = 0;
    while (i < limit) {
      IO.Write(i, 3);
      IO.WriteLine(fib(i), 10);
      i++;
    }
  } // Main

} // Fibo
