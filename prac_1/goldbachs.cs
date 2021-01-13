using System;
using Library;

public class Goldbachs
{
    public static IntSet Sieve (int n)
    {
        IntSet primeSet = new IntSet();
        int i, k;                                           // counters
        for (i = 2; i <= n; i++) primeSet.Incl(i);          // populate set with 2 .. n
        for (i = 2; i <= n; i++)
        {
            if (primeSet.Contains(i))                       // does set contain i?
            {
                k = i + i;                                  // increase counter k by 2i so the value in the set remains as the prime
                do                                          
                {
                    primeSet.Excl(k);                       // exclude all multiples of i from set
                    k += i;
                } while (k <= n);
            }
        }
        return primeSet;
    }

    public static void findConjecture(IntSet p, int n)       // similar to the sieve method above
    {
        IntSet primeSet = new IntSet();
        int i, k;                                           
        for (i = 2; i <= n; i++) primeSet.Incl(i);
        for (i = 2; i <= n; i++)
        {
            if (primeSet.Contains(i))                       // we know if this is true the number is prime
            {
                if (p.Contains(n - i))                        // since B = N - A, we can check if the prime number minus n (i - n) is a prime
                {
                    IO.WriteLine(n + " = " + i + " + " + (n - i));                              // (i - n) is a prime hence we have found the conjecture.
                }
                k = i + i;
                do
                {
                    primeSet.Excl(k);
                    k += i;
                } while (k <= n);
            }
        }
    }

    public static void Main(String[] args) {
        IO.Write("Enter n, the upper bound to calculate the goldbachs.");
        int n = IO.ReadInt();                                                           // get n
        if (n < 4)                                                          
        {
            IO.WriteLine("Please give an n larger than or equal to 4...");
        }
        IO.WriteLine("Even numbers N between 4 and " + n + " such that N = A + B");
        IO.WriteLine("-----------------------------------");

        IntSet primes = Sieve(n);                                                       // find all the primes between 2 and upper bound n

        IO.WriteLine("N = A + B");
        for (int i = 4; i <= n; i = i + 2)                                              // loop between all even numbers between 4 .. N
        {
            int a = findConjecture(primes, i);                                          // find conjecture for some even number between 4 .. N
        }

    }
}
