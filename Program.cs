using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace DSA
{
    internal static class Program
    {
        private static int Q { get; set; }
        private static int P { get; set; }
        private static BigInteger G { get; set; }
        private static int H { get; set; }
        private static int X { get; set; }
        private static BigInteger Y { get; set; }
        private static int K { get; set; }
        private static BigInteger R { get; set; }
        private static BigInteger S { get; set; }
        private static BigInteger W { get; set; }
        private static BigInteger U1 { get; set; }
        private static BigInteger U2 { get; set; }
        private static BigInteger V { get; set; }


        private static void Main()
        {
            H = 15;
            Console.Write("Introduce the text to hash: ");
            var textToHash = Console.ReadLine();
            var hashedText = Hash(textToHash);
            Console.WriteLine("Hashed text: {0}", hashedText);

            H = FoldString(hashedText, 4);
            Console.WriteLine("H: {0}", H);

            List<int> primeNumbers = GenerateFirstNPrimeNumbers(25);
            
            Q = primeNumbers[GetRandomNumber(primeNumbers.Count - 1)];
            Console.WriteLine("Q: {0}", Q);
            
            // Get P
            while (true)
            {
                var z = GetRandomNumberFromInterval(Q, 5 * Q);
            
                P = 2 * Q * z + 1;
            
                if (IsPrime(P)) break;
            }
            
            Console.WriteLine("P: {0}", P);
            
            // Get G
            var possibleGs = new List<BigInteger>();
            
            for (var i = 2; i <= P - 2; i++) possibleGs.Add(BigInteger.Pow(i, (P - 1) / Q) % P);

            possibleGs = possibleGs
                .Distinct()
                .Where(l => l != 1)
                .ToList();
            
            G = possibleGs[GetRandomNumber(possibleGs.Count - 1)];
            Console.WriteLine("G: {0}", G);

            X = GetRandomNumberFromInterval(1, Q - 1);
            Console.WriteLine("X: {0}", X);

            Y = BigInteger.Pow(G, X) % P;
            Console.WriteLine("Y: {0}", Y);

            // Get R and S
            while (true)
            {
                K = GetRandomNumberFromInterval(1, Q - 1);

                R = BigInteger.Pow(G, K) % P % Q;
                
                if (R == 0) continue;

                var temp2 = BigInteger.Pow(K, Q - 2) % Q;
                S = temp2 * (H + X * R) % Q;

                if (S != 0) break;
            }
            
            Console.WriteLine("R: {0}", R);
            Console.WriteLine("S: {0}", S);

            W = BigInteger.Pow(S, Q - 2) % Q;
            Console.WriteLine("W: {0}", W);

            U1 = H * W % Q;
            Console.WriteLine("U1: {0}", U1);

            U2 = R * W % Q;
            Console.WriteLine("U2: {0}", U2);

            // Calculate V
            BigInteger prod1 = 1;
            for (var i = 1; i <= U1; i++) prod1 *= G;

            BigInteger prod2 = 1;
            for (var i = 1; i <= U2; i++) prod2 *= Y;

            var temp = prod1 * prod2 % P;

            V = temp % Q;
            Console.WriteLine("V: {0}", V);
            Console.WriteLine(V == R ? "Semnatura este valida" : "Semnatura este invalida");
        }
        
        private static List<int> GenerateFirstNPrimeNumbers(int n)
        {
            var result = new List<int>();

            var i = 2;

            while (result.Count < n)
            {
                if (IsPrime(i)) result.Add(i);
                
                i = i == 2 ? i + 1 : i + 2;
            }
            
            return result;
        }
        
        private static bool IsPrime(long n)
        {
            for (var i = 2; i <= n / 2; i++)
            {
                if (n % i == 0) return false;
            }

            return true;
        }
        
        private static int GetRandomNumber(int max) => new Random().Next(max);
        
        private static int GetRandomNumberFromInterval(int from, int to) => new Random().Next(from ,to);

        private static string Hash(string text)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            { 
                byte[] sourceBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-",String.Empty);
                
                return hash;
            }
        }

        private static int FoldString(string s, int m)
        {
            long sum = 0, mul = 1;
            for (int i = 0; i < s.Length; i++) {
                mul = (i % 4 == 0) ? 1 : mul * 256;
                sum += s[i] * mul;
            }
            return (int)(Math.Abs(sum));
        }
    }
}
