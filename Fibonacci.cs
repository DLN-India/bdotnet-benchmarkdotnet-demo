using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace
    Benchmark
{
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true), MemoryDiagnoser]
    public class FibonacciCalc
    {
        // 1. Notice the implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Added MemoryDiagnoser to the benchmark
        // 3. Run with release configuration and show the results
        // 4. Open disassembler report and compare machine code

        [Benchmark, ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        private Dictionary<ulong, ulong> _memo = new Dictionary<ulong, ulong>();

        [Benchmark, ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoization(ulong n)
        {
            //Build up a memTable
            // Base case: 0 or 1
            if (n == 0 || n == 1)
            {
                return n;
            }

            // See if we've already calculated this
            ulong result;
            if (!_memo.TryGetValue(n, out result))
            {
                // Not yet, so compute it
                result = RecursiveWithMemoization(n - 1) + RecursiveWithMemoization(n - 2);

                // Memoize
                _memo.Add(n, result);
            }

            return result;
        }

        [Benchmark(Baseline = true), ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Index was negative. No such thing as a negative index in a series.");
            }

            if (n == 0 || n == 1)
            {
                return n;
            }

            // We'll be building the fibonacci series from the bottom up.
            // So we'll need to track the previous 2 numbers at each step.
            ulong prevPrev = 0;  // 0th fibonacci
            ulong prev = 1;      // 1st fibonacci
            ulong current = 0;   // Declare and initialize current

            for (ulong i = 1; i < n; i++)
            {
                // Iteration 1: current = 2nd fibonacci
                // Iteration 2: current = 3rd fibonacci
                // Iteration 3: current = 4th fibonacci
                // To get nth fibonacci ... do n-1 iterations.
                current = prev + prevPrev;
                prevPrev = prev;
                prev = current;
            }

            return current;
        }

        public IEnumerable<ulong> Data()
        {
            yield return 15;
            yield return 35;
        }
    }
}