using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_SWEN1_WS2021
{
    public interface ICard
    {
        public const int upgradeDamage = 10;
        public enum Elements
        {
            Undefined = -1,
            Water = 0,
            Fire = 1,
            Normal = 2
        }

        Elements Element { get; set; }
        public int Id { get; set; }
        string Name { get; }
        int Damage { get; }
        int Upgrade { get; }

        int attack(ICard Enemy);

        static float[,] Effectiveness = new float[,] { { 1, 2, 0.5f }, { 0.5f, 1, 2 }, { 2, 0.5f, 1 } };

    }
}
