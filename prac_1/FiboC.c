// Print a table of Fibonacci numbers using (slow) recursive definition
// P.D. Terry,  Rhodes University, 2017

#include "misc.h" 

  int fib(int m) {
  // Compute m-th term in Fibonacci series 0,1,1,2 ...
    if (m == 0) return 0;
    if (m == 1) return 1;
    return fib(m-1) + fib(m-2);
  } // fib

  void main() {
    int i, limit;
    printf("Supply upper limit ");
    scanf("%d", &limit);
    i = 0;
    while (i < limit) {
      printf("%5d %10d\n", i, fib(i));
      i++;
    }
  } // main
