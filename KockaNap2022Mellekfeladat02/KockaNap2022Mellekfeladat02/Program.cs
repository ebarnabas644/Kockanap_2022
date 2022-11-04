using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KockaNap2022Mellekfeladat02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> area = GetInput();

            Console.WriteLine(Calculate(area));
            Console.ReadKey();
        }

        static int Calculate(List<string> list)
        {
            if (list.Count == 1)
            {
                return 0;
            }

            int res = 0;

            for (int i = 1; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Length; j++)
                {
                    int x = i;

                    while(x >= 0 && list[x][j] != 'O')
                    {
                        x--;
                    }
                    if (x >= 0 && x != i)
                    {
                        res++;
                    }
                }
            }
            return res;

        }
        static List<string> GetInput()
        {
            List<string> map = new List<string>();
            string sor = Console.ReadLine();
            do
            {
                map.Add(sor);
                sor = Console.ReadLine();
                
            } while (sor != "");

            return map;
        }
    }
}
