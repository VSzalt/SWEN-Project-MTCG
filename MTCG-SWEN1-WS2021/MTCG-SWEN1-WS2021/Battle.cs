using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MTCG_SWEN1_WS2021
{
    public class Battle
    {
        public Battle(User player, List<ICard> cpuDeck)
        {
            Player = player;
            PlayerDeck = new List<ICard>(player.Deck);
            CPUDeck = cpuDeck;
        }

        const int maxRounds = 100;

        public User Player { get; }
        public List<ICard> PlayerDeck { get; }
        public List<ICard> CPUDeck { get; }

        public void Run()
        {
            for (int i = 0; i < maxRounds; i++)
            {
                PlayRound(pickRandomCard(PlayerDeck), pickRandomCard(CPUDeck));

                if (PlayerDeck.Count == 0 || CPUDeck.Count == 0)
                    break;
            }
            if(PlayerDeck.Count == 0)
            {
                Console.WriteLine("CPU won the game!");
                Player.ELO -= 5;
                Console.WriteLine("You lost 5 points for the scoreboard!");
            }
            else if(CPUDeck.Count == 0)
            {
                Console.WriteLine("Player won the game!");
                Player.ELO += 3;
                Player.Coins += 1;
                Console.WriteLine("You earned 3 points for the scoreboard and a coin!");
            }
            else
            {
                Console.WriteLine("100 Rounds played! Match ends in a draw!");
            }
            Player.GamesPlayed += 1;
        }

        public void PlayRound(ICard playerCard, ICard cpuCard)
        {
            int FinalDamage1 = playerCard.attack(cpuCard);
            int FinalDamage2 = cpuCard.attack(playerCard);
            Console.WriteLine($"Player: {playerCard.Name} ({FinalDamage1} Damage) vs CPU: {cpuCard.Name} ({FinalDamage2} Damage)");
            Thread.Sleep(500);
            if (FinalDamage1 == FinalDamage2)
            {
                Console.WriteLine("Battle ends in a draw");
            }

            else if (FinalDamage1 > FinalDamage2)
            {
                PlayerDeck.Add(cpuCard);
                CPUDeck.Remove(cpuCard);
                Console.WriteLine($"Player wins the battle with {playerCard.Name}!");
            }
            else
            {
                CPUDeck.Add(playerCard);
                PlayerDeck.Remove(playerCard);
                Console.WriteLine($"CPU wins the battle with {cpuCard.Name}!");
            }
            Console.WriteLine();
            Thread.Sleep(1000);
        }

        public virtual ICard pickRandomCard(List<ICard> deck)
        {
            var random = new Random();
            int index = random.Next(deck.Count);

            return deck[index];
        }
    }
}
