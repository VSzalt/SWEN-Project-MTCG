using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_SWEN1_WS2021
{
    public class User
    {
        public string Username { get; }
        public List<ICard> Stack = new List<ICard>();
        public List<ICard> Deck = new List<ICard>();
        public int Coins = 20;
        public int ELO = 100;
        public int GamesPlayed = 0;

        public User(string username)
        {
            Username = username;       
        }
        public User(string username, int coins, int elo, int gamesPlayed)
        {
            Username = username;
            Coins = coins;
            ELO = elo;
            GamesPlayed = gamesPlayed;
        }
        public void test()
        {
            Console.WriteLine();
            Console.WriteLine("Login erfolgreich!");
        }
        public void ManageCards()
        {
            var oldDeck = Deck;
            Deck = new List<ICard>();
            for(int i = 0; i < Stack.Count(); i++)
            {
                Console.WriteLine($"{i} {Stack[i].Name}");
                
            }
            Console.WriteLine();
            Console.WriteLine("Geben Sie 4 verschiedene Zahlen mit den gewünschten Karten getrennt durch Leerzeichen ein!");         

            try
            {
                Console.ReadLine()
                    .Split()
                    .Select((number) => Int32.Parse(number))
                    .Where((number) => 0 <= number && number < Stack.Count())
                    .Distinct()//Unique
                    .ToList()
                    .ForEach(index => Deck.Add(Stack[index]));

                if (Deck.Count != 4)
                    throw new Exception("Ein Deck muss 4 Karten beinhalten!");                
            }
            catch
            {
                Console.WriteLine("Geben Sie nur 4 (verschiedene) Zahlen ein!");
                Deck = oldDeck;                
            }
            Console.WriteLine("");
            Console.WriteLine("Current Deck:");
            for (int i = 0; i < Deck.Count(); i++)
            {
                Console.WriteLine($"{i} {Deck[i].Name}");

            }
        }
    }
}
