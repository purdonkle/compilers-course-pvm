// Sieve of Eratosthenes for finding primes 2 <= n <= Max
// P.D. Terry,  Rhodes University, 2017

#include <iostream>
#include "misc.h"

using namespace std;

  const int Max = 32000;
  typedef boolean SIEVES[Max - 1];

  int main( ) {
    int i, n, k, it, iterations, primes;         // counters
    SIEVES uncrossed;                            // the sieve
    boolean display;
    cout << "How many iterations? ";
    cin >> iterations;
    display = iterations == 1;
    cout << "Supply largest number to be tested ";
    cin >> n;
    if (n > Max) {
      cout << "n too large, sorry";
      exit(1);
    }
    cout << "Prime numbers between 2 and " << n << "\n";
    cout << "-----------------------------------\n";
    for (it = 1; it <= iterations; it++) {
      primes = 0;
      for (i = 2; i <= n; i++)                   // clear sieve
        uncrossed[i] = true;
      for (i = 2; i <= n; i++)                   // the passes over the sieve
        if (uncrossed[i]) {
          if (display && primes % 8 == 0) cout << '\n';     // ensure line not too long
          primes++;
          if (display) cout << "\t" << i;
          k = i;                                 // now cross out multiples of i
          do {
            uncrossed[k] = false;
            k += i;
          } while (k <= n);
        }
      if (display) cout << "\n";
    }
    cout << primes << " primes";
  } // main
