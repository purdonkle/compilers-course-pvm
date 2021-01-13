using Library;
using System;

namespace SieveIntSet {
    class SieveWithIntSet
    {
        public static IntSet Sieve (int n) {
            IntSet primeSet = new IntSet();
            int i, k;                                           // counters
            for (i = 2; i <= n; i++) primeSet.Incl(i);          // populate set with 2 .. n
            for (i = 2; i <= n; i++)
            {
                if (primeSet.Contains(i))                       // does set contain i?
                {                                             
                    k = i + i;                                  // increase counter k by 2i so the value in the set remains as the prime
                    do                                          // exclude all multiples of i from set
                    {
                        primeSet.Excl(k);                       
                        k += i;
                    } while (k <= n);
                }
            }
            return primeSet;
        }


        public static void Main(string[] args)
        {
            IO.Write("Enter n, the upper bound to calculate.");
            int n = IO.ReadInt();                               // get n
            IO.WriteLine("Prime numbers between 2 and " + n);
            IO.WriteLine("-----------------------------------");
            IntSet primeSet = Sieve(n);                         // get primes
            primeSet.Write();                                   // output primes
            IO.Write("\nprimes: " + primeSet.Members());        // Print number primes
        }
    }
}
