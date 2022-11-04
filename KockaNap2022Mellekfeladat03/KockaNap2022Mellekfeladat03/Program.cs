using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KockaNap2022Mellekfeladat03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int m = int.Parse(Console.ReadLine());
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(Math.Round(Calculate(m,n,100000),2).ToString("0.0"));
        }

        private static double Calculate(int M, int N, int it)
        {
            int s = 0;
            int n = 0;
            int m = 0;
            for (int i = 0; i < it; i++)
            {
                m += M;
                if (n < m)
                {
                    n += N;
                }
                s += n - m;
            }
            return (double)s / it;
        }
    }
}
