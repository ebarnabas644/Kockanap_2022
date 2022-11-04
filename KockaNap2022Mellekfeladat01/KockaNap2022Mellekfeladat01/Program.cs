using System;
using System.Collections;
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
        private string name;
        private List<int> _starPowers;

        private int maxStarPower = 0;

        public StarSystem(string name, List<int> starPowers)
        {
            this.name = name;
            this._starPowers = starPowers;
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
            List<int> stas = new List<int>();
            string[] powes = input[input.Length - 1].Split(';');
            foreach (var power in powes)
            {
                stas.Add(int.Parse(power));
            }
            listStarsystem.Add(new StarSystem(input[input.Length - 2].Substring(1, input[input.Length - 2].Length - 2), stas));


            return listStarsystem;
        }
        public static void CalculateAll(List<StarSystem> starSystems)
        {
            foreach (var system in starSystems)
            {
                StarSystem.CalculatePower(system);
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

        public static void CalculatePower(StarSystem system)
        {
            while (system._starPowers.Count > 2)
            {
                List<int> _maxs = new List<int>();
                for (int i = 1; i < system._starPowers.Count - 1; i++)
                {
                    _maxs.Add(system._starPowers[i - 1] * system._starPowers[i + 1]);
                }
                //int maxValue = _maxs.Max();
                int idx = _maxs.IndexOf(_maxs.Max());
                if (_maxs.Count > 1)
                {
                    for (int i = 0; i < _maxs.Count; i++)
                    {
                        if (_maxs[i] == _maxs[idx] && system._starPowers[i + 1] < system._starPowers[idx + 1])
                        {
                            idx = i;
                        }
                    }
                }
                system.maxStarPower += _maxs[idx];
                system._starPowers.RemoveAt(idx);
            }
        }


        public override string ToString()
        {
            return string.Format("{0}: {1}", name, maxStarPower);
        }
    }
}
