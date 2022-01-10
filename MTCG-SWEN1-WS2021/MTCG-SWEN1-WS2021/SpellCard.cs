using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_SWEN1_WS2021
{
    public class SpellCard : ICard
    {
        public SpellCard(int id, ICard.Elements element, string name, int damage, int upgrade)
        {
            Id = id;
            Element = element;
            Name = name;
            Damage = damage + (upgrade * ICard.upgradeDamage);
            Upgrade = upgrade;
        }

        public SpellCard(int id) : this(id, ICard.Elements.Undefined, "placeholder", 0, 0) { }

        public ICard.Elements Element { get; set; }
        public int Id { get; set; }
        public string Name { get; }
        public int Damage { get; }
        public int Upgrade { get; }

        public int attack(ICard Enemy)
        {
            if (Enemy is MonsterCard)
            {
                var monsterEnemy = (MonsterCard)Enemy;
                if (monsterEnemy.Race == MonsterCard.Species.Kraken)
                {
                    return 0;
                }
            }
            var effectiveness = ICard.Effectiveness[((int)this.Element), ((int)Enemy.Element)];
            return (int)(Damage * effectiveness);
        }
    }
}
