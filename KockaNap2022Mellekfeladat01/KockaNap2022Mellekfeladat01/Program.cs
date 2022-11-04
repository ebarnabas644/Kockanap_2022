using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KockaNap2022Mellekfeladat01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] file = File.ReadAllLines("starsystem.in");
            List<StarSystem> starSystems = StarSystem.GetInput(file);

            StarSystem.CalculateAll(starSystems);


            File.WriteAllLines("starsystem.out", StarSystem.ToStringAll(starSystems));


            Console.ReadKey();
        }

        
    }

    internal class StarSystem
    {
        private string _name;
        private List<int> _starPowers;

        private int _maxStarPower = 0;

        public StarSystem(string name, List<int> starPowers)
        {
            _name = name;
            _starPowers = starPowers;
        }

        public static List<StarSystem> GetInput(string[] input)
        {
            List<StarSystem> listStarsystem = new List<StarSystem>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == "")
                {

                    List<int> stars = new List<int>();
                    string[] powers = input[i - 1].Split(';');
                    foreach (var power in powers)
                    {
                        stars.Add(int.Parse(power));
                    }

                    listStarsystem.Add(new StarSystem(input[i - 2].Substring(1, input[i - 2].Length - 2), stars));
                }
            }

            return listStarsystem;
        }
        public static void CalculateAll(List<StarSystem> starSystems)
        {
            foreach (var systems in starSystems)
            {
                systems.CalculatePower();
            }
        }

        public static string[] ToStringAll(List<StarSystem> starSystems)
        {
            List<string> systems = new List<string>();

            foreach (var system in starSystems)
            {
                systems.Add(system.ToString());
            }
            return systems.ToArray();
        }

        public void CalculatePower()
        {

        }


        public override string ToString()
        {
            return string.Format("{0}: {1}", _name, _maxStarPower);
        }
    }
}
