using NUnit.Framework;
using MTCG_SWEN1_WS2021;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace MTCG_SWEN1_WS2021.Tests
{
    [TestFixture]
    public class BattleTests
    {
        MonsterCard waterMonster;
        MonsterCard fireMonster;
        MonsterCard normalMonster;

        SpellCard waterSpell;
        SpellCard fireSpell;
        SpellCard normalSpell;

        MonsterCard goblin;
        MonsterCard ork;
        MonsterCard fireElf;
        MonsterCard knight;
        MonsterCard wizard;
        MonsterCard kraken;
        MonsterCard dragon;

        User user;

        int EffectiveDamage = 2;
        float NonEffectiveDamage = 0.5f;
        int ImmuneDamage = 0;
        int ELOAfterWin = 103;
        int ELOAfterLoss = 95;
        int CoinsAfterWin = 21;

        [SetUp]
        public void Init()
        {
            waterMonster = new MonsterCard(1,ICard.Elements.Water, "WaterGoblin", 20, MonsterCard.Species.Goblin, 0);
            fireMonster = new MonsterCard(2,ICard.Elements.Fire, "FireGoblin", 20, MonsterCard.Species.Goblin, 0);
            normalMonster = new MonsterCard(3,ICard.Elements.Normal, "NormalGoblin", 20, MonsterCard.Species.Goblin, 0);

            waterSpell = new SpellCard(1,ICard.Elements.Water, "WaterSpell", 20, 0);
            fireSpell = new SpellCard(2,ICard.Elements.Fire, "FireSpell", 20, 0);
            normalSpell = new SpellCard(3,ICard.Elements.Normal, "NormalSpell", 20, 0);

            goblin = new MonsterCard(3,ICard.Elements.Normal, "Goblin", 15, MonsterCard.Species.Goblin, 0);
            ork = new MonsterCard(6,ICard.Elements.Normal, "Ork", 60, MonsterCard.Species.Ork, 0);
            fireElf = new MonsterCard(8,ICard.Elements.Fire, "Elf", 30, MonsterCard.Species.Elf, 0);
            knight = new MonsterCard(12,ICard.Elements.Normal, "Knight", 55, MonsterCard.Species.Knight, 0);
            wizard = new MonsterCard(15,ICard.Elements.Normal, "Wizard", 50, MonsterCard.Species.Wizard, 0);
            kraken = new MonsterCard(18,ICard.Elements.Normal, "Kraken", 75, MonsterCard.Species.Kraken, 0);
            dragon = new MonsterCard(21,ICard.Elements.Normal, "Dragon", 100, MonsterCard.Species.Dragon, 0);

            user = new User("test", 20, 100, 0); 
        }

        //attack Tests
        [Test]
        public void MonsterAttacksMonsterResultsInBaseDamage()
        {
            int expectedDamage = waterMonster.Damage;
            int damage = waterMonster.attack(waterMonster);            
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void MonsterAttacksSpellResultsInBaseDamage()
        {
            int expectedDamage = waterMonster.Damage;
            int damage = waterMonster.attack(waterSpell);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void MonsterAttacksSpellResultsInEffectiveDamage()
        {
            int expectedDamage = waterMonster.Damage * EffectiveDamage;
            int damage = waterMonster.attack(fireSpell);
            Assert.AreEqual(expectedDamage, damage);
        }

        [Test]
        public void MonsterAttacksSpellResultsInNonEffectiveDamage()
        {
            float expectedDamage = waterMonster.Damage * NonEffectiveDamage;
            float damage = waterMonster.attack(normalSpell);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void SpellAttacksSpellResultsInBaseDamage()
        {
            int expectedDamage = waterSpell.Damage;
            int damage = waterSpell.attack(waterSpell);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void SpellAttacksMonsterResultsInBaseDamage()
        {
            int expectedDamage = waterSpell.Damage;
            int damage = waterSpell.attack(waterMonster);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void SpellAttacksMonsterResultsInEffectiveDamage()
        {
            int expectedDamage = waterSpell.Damage * EffectiveDamage;
            int damage = waterSpell.attack(fireMonster);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void SpellAttacksMonsterResultsInNonEffectiveDamage()
        {
            float expectedDamage = waterSpell.Damage * NonEffectiveDamage;
            float damage = waterSpell.attack(normalMonster);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void GoblinAttacksDragonResultsInNoDamage()
        {
            float expectedDamage = ImmuneDamage;
            float damage = goblin.attack(dragon);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void OrkAttacksWizardResultsInNoDamage()
        {
            float expectedDamage = ImmuneDamage;
            float damage = ork.attack(wizard);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void KnightAttacksWaterSpellResultsInNoDamage()
        {
            float expectedDamage = ImmuneDamage;
            float damage = knight.attack(waterSpell);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void SpellAttacksKrakenResultsInNoDamage()
        {
            float expectedDamage = ImmuneDamage;
            float damage = waterSpell.attack(kraken);
            Assert.AreEqual(expectedDamage, damage);
        }
        [Test]
        public void DragonAttacksFireElfResultsInNoDamage()
        {
            float expectedDamage = ImmuneDamage;
            float damage = dragon.attack(fireElf);
            Assert.AreEqual(expectedDamage, damage);
        }

        //PlayRound Tests
        [Test]
        public void Card1LosesAndMovesIntoWinningDeckAfterPlayedRound()
        {
            user.Deck = new List<ICard> { fireMonster, fireMonster };
            List<ICard> deck2 = new List<ICard> { waterSpell, waterSpell };

            Battle testBattle = new Battle(user, deck2);

            testBattle.PlayRound(testBattle.PlayerDeck[0], deck2[0]);

            Assert.That(testBattle.PlayerDeck, Has.Exactly(1).Items);
            Assert.That(deck2, Has.Exactly(3).Items);
        }
        [Test]
        public void Card2LosesAndMovesIntoWinningDeckAfterPlayedRound()
        {
            user.Deck = new List<ICard> { waterSpell, waterSpell };
            List<ICard> deck2 = new List<ICard> { fireMonster, fireMonster };

            Battle testBattle = new Battle(user, deck2);

            testBattle.PlayRound(testBattle.PlayerDeck[0], deck2[0]);

            Assert.That(testBattle.PlayerDeck, Has.Exactly(3).Items);
            Assert.That(deck2, Has.Exactly(1).Items);
        }
        [Test]
        public void NoCardMovesAfterDrawInPlayedRound()
        {
            user.Deck = new List<ICard> { normalSpell, waterSpell };
            List<ICard> deck2 = new List<ICard> { normalMonster, fireMonster };

            Battle testBattle = new Battle(user, deck2);

            testBattle.PlayRound(testBattle.PlayerDeck[0], deck2[0]);

            Assert.That(testBattle.PlayerDeck, Has.Exactly(2).Items);
            Assert.That(deck2, Has.Exactly(2).Items);
        }

        //Run Tests
        [Test]
        public void Deck1HasNoCardsAfterLosingAllBattles()
        {
            
            user.Deck = new List<ICard> { fireMonster, fireMonster };
            List<ICard> deck2 = new List<ICard> { waterSpell, waterSpell };

            Mock<Battle> mockedBattle = new Mock<Battle>(user, deck2);
            mockedBattle.CallBase = true;
            mockedBattle.Setup(x => x.pickRandomCard(It.IsAny<List<ICard>>())).Returns((List<ICard> x) => { return x[0]; });

            mockedBattle.Object.Run();

            Assert.That(mockedBattle.Object.PlayerDeck, Has.Exactly(0).Items);
            Assert.That(deck2, Has.Exactly(4).Items);
        }
        [Test]
        public void NoCardsMoveAfterAllBattles()
        {

            user.Deck = new List<ICard> { normalMonster, normalMonster };
            List<ICard> deck2 = new List<ICard> { normalSpell, normalSpell };

            Mock<Battle> mockedBattle = new Mock<Battle>(user, deck2);
            mockedBattle.CallBase = true;
            mockedBattle.Setup(x => x.pickRandomCard(It.IsAny<List<ICard>>())).Returns((List<ICard> x) => { return x[0]; });

            mockedBattle.Object.Run();

            Assert.That(mockedBattle.Object.PlayerDeck, Has.Exactly(2).Items);
            Assert.That(deck2, Has.Exactly(2).Items);
        }
        //Rewards after Run Tests
        [Test]
        public void ELOIncreasesAfterWonBattle()
        {
            int expectedELO = ELOAfterWin;
            user.Deck = new List<ICard> { fireMonster, fireMonster };
            List<ICard> deck2 = new List<ICard> { normalSpell, normalSpell };

            Mock<Battle> mockedBattle = new Mock<Battle>(user, deck2);
            mockedBattle.CallBase = true;
            mockedBattle.Setup(x => x.pickRandomCard(It.IsAny<List<ICard>>())).Returns((List<ICard> x) => { return x[0]; });

            mockedBattle.Object.Run();

            Assert.AreEqual(user.ELO, expectedELO);
        }
        [Test]
        public void ELODecreasesAfterLostBattle()
        {
            int expectedELO = ELOAfterLoss;
            user.Deck = new List<ICard> { fireMonster, fireMonster };
            List<ICard> deck2 = new List<ICard> { waterSpell, waterSpell };

            Mock<Battle> mockedBattle = new Mock<Battle>(user, deck2);
            mockedBattle.CallBase = true;
            mockedBattle.Setup(x => x.pickRandomCard(It.IsAny<List<ICard>>())).Returns((List<ICard> x) => { return x[0]; });

            mockedBattle.Object.Run();

            Assert.AreEqual(user.ELO, expectedELO);
        }
        [Test]
        public void CoinsIncreaseAfterWonBattle()
        {
            int expectedELO = CoinsAfterWin;
            user.Deck = new List<ICard> { fireMonster, fireMonster };
            List<ICard> deck2 = new List<ICard> { normalSpell, normalSpell };

            Mock<Battle> mockedBattle = new Mock<Battle>(user, deck2);
            mockedBattle.CallBase = true;
            mockedBattle.Setup(x => x.pickRandomCard(It.IsAny<List<ICard>>())).Returns((List<ICard> x) => { return x[0]; });

            mockedBattle.Object.Run();

            Assert.AreEqual(user.Coins, CoinsAfterWin);
        }
    }
}