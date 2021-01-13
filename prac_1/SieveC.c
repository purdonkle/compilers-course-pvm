// Sieve of Eratosthenes for finding primes 2 <= n <= Max
// P.D. Terry,  Rhodes University, 2017 

  #include "misc.h"

  int Max = 32000;
  typedef boolean SIEVES[32000];

  void main(void) {
    int i, n, k, it, iterations, primes;         // counters
    SIEVES uncrossed;
    boolean display;                             // the sieve
    printf("How many iterations? ");
    scanf("%d", &iterations);
    display = iterations == 1;
    printf("Supply largest number to be tested ");
    scanf("%d", &n);
    if (n > Max) {
      printf("n too large, sorry");
      exit(1);
    }
    printf("Prime numbers between 2 and %d\n", n);
    printf("-----------------------------------\n");
    for (it = 1; it <= iterations; it++) {
      primes = 0;
      for (i = 2; i <= n; i++)                   // clear sieve
        uncrossed[i] = true;
      for (i = 2; i <= n; i++)                   // the passes over the sieve
        if (uncrossed[i]) {
          if (display && primes % 10 == 0) putchar('\n');   // ensure line not too long
          primes++;
          if (display) printf("%6d", i);
          k = i;                                 // now cross out multiples of i
          do {
            uncrossed[k] = false;
            k += i;
          } while (k <= n);
        }
      if (display) printf("\n");
    }
    printf("%d primes", primes);
  }  // main
