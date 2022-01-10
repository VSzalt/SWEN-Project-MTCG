using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_SWEN1_WS2021
{
    public class MonsterCard : ICard
    {
        public enum Species
        {
            Goblin = 0,
            Ork = 1,
            Elf = 2,
            Knight = 3,
            Wizard = 4,
            Kraken = 5,
            Dragon = 6
        }
        public MonsterCard(int id, ICard.Elements element, string name, int damage, Species species, int upgrade)
        {
            Id = id;
            Element = element;
            Name = name;
            Damage = damage + (upgrade * ICard.upgradeDamage);
            Race = species;
            Upgrade = upgrade;
        }

        public MonsterCard(int id) : this(id, ICard.Elements.Undefined, "placeholder", 0, Species.Goblin , 0) { }

        public ICard.Elements Element { get; set; }
        public int Id { get; set; }
        public string Name { get; }
        public int Damage { get; }
        public Species Race { get; set; }
        public int Upgrade { get; }

        public int attack(ICard Enemy)
        {
            if(Enemy is MonsterCard)
            {
                var monsterEnemy = (MonsterCard)Enemy;
                if(this.Race == Species.Goblin && monsterEnemy.Race == Species.Dragon)
                {
                    return 0;
                }
                if(this.Race == Species.Ork && monsterEnemy.Race == Species.Wizard)
                {
                    return 0;
                }
                if(this.Race == Species.Dragon && monsterEnemy.Race == Species.Elf && monsterEnemy.Element == ICard.Elements.Fire)
                {
                    return 0;
                }
                return Damage;
            }
            
            if(Enemy is SpellCard)
            {
                var spellEnemy = (SpellCard)Enemy;

                if(this.Race == Species.Knight && spellEnemy.Element == ICard.Elements.Water)
                {
                    return 0;
                }
                var effectiveness = ICard.Effectiveness[((int)this.Element),((int)Enemy.Element)];
                return (int)(Damage * effectiveness);
            }
            return Damage;
        }
    }
}
