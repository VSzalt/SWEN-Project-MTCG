using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace MTCG_SWEN1_WS2021
{
    public class Program
    {
        static void Main(string[] args)
        {
            var database = new Database();
            var allMonsters = database.getAllMonsters();
            var allSpells = database.getAllSpells();
            int packagePrice = 5;
            var minimumPasswordLength = 4;

            Console.WriteLine("Willkommen zu MTCG!");

            char input = ' ';
            User user = null;
            do
            {
                //Menu-Printing
                Console.WriteLine();
                Console.WriteLine("Wählen Sie einen der folgenden Optionen aus:");
                Console.WriteLine();
                if (user == null)
                {
                    Console.WriteLine("'l' für Login");
                    Console.WriteLine("'r' für Registrierung");
                    Console.WriteLine("'q' für das Beenden des Programms");
                }
                else
                {
                    Console.WriteLine("'d' für Kartensammlung & Deckzusammenstellung");
                    Console.WriteLine("'s' für Shop");
                    Console.WriteLine("'t' für Trade anfragen");
                    Console.WriteLine("'a' für alle Trading-Angebote");
                    Console.WriteLine("'b' für Battle");
                    Console.WriteLine("'p' für Profilansicht");
                    Console.WriteLine("'e' für Scoreboard (ELO-Score)");
                    Console.WriteLine("'q' für das Beenden des Programms");
                }
                

                input = Console.ReadKey().KeyChar;
                Console.WriteLine();
                switch (input)
                {
                    case 'l':
                        //Login
                        Console.WriteLine();                        
                        user = login();
                        
                        if (user != null)
                        {
                            user.test();
                            break;
                        }
                        else
                        {                         
                            Console.WriteLine("Die Logindaten waren leider nicht korrekt!");
                            user = null;                                                    
                            break;
                        }

                    case 'd':
                        // Kartensammlung + Deckzusammenstellung
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }

                        user.Stack = database.getAllUserCards(user);
                        Console.WriteLine("Hier sehen Sie ihre Kartensammlung!");
                        Console.WriteLine();
                        user.ManageCards();
                        break;

                    case 's':
                        // Shop
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }
                        if (user.Coins < packagePrice)
                        {
                            Console.WriteLine("Ups! Du hast nicht genug Münzen! Du brauchst mindestens 5 Münzen!");
                            break;
                        }
                        shop();
                        break;

                    case 't':
                        // Trade anfragen
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }

                        user.Stack = database.getAllUserCards(user);
                        if (database.isUserTrading(user))
                        {
                            var card = database.getCardFromActiveTrade(user);
                            database.setLockState(user, card, false);
                            withdrawTrade();
                        }
                        else
                        {
                            trading();
                        }

                        break;

                    case 'a':
                        // Trading-Angebote
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }
                        user.Stack = database.getAllUserCards(user);
                        if (tradeOffers())
                            Console.WriteLine("Trade erfolgreich!");
                        break;

                    case 'b':
                        // CPU-Generieren + Battlelogik
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }
                        if (user.Deck.Count == 0)
                        {
                            Console.WriteLine("Ups! Du musst ein Deck vorher zusammenstellen!");
                            break;
                        }
                        //Liste mit allen Karten
                        var cards = new List<ICard>(allMonsters);
                        cards.AddRange(allSpells);
                        var random = new Random();
                        int getRange = 4;
                        var shuffledCards = cards.OrderBy((card) => random.Next()).Take(getRange).ToList();

                        var battle = new Battle(user, shuffledCards);
                        battle.Run();
                        Console.WriteLine($"Gespielte Battles: {user.GamesPlayed}");
                        database.updateUser(user);
                        break;

                    case 'p':
                        //Profilansicht + Stats
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }
                        profile();
                        break;

                    case 'e':
                        // Scoreboard (ELO-Score)
                        Console.WriteLine();
                        if (user == null)
                        {
                            Console.WriteLine("Ups! Du musst dich mit deinem Account einloggen!");
                            break;
                        }
                        printELO(database.getScoreboard());
                        break;

                    case 'r':
                        //Registrieren
                        Console.WriteLine();
                        register();
                        break;
                }
            } while (input != 'q');

            User login()
            {
                Console.WriteLine("Geben Sie Ihren Username ein!");
                var username = Console.ReadLine();
                if(username.Length <= 0)
                {
                    Console.WriteLine("Der Benutzername darf nicht leer sein!");
                    return null;
                }
                Console.WriteLine("Geben Sie Ihr Passwort ein!");
                var password = GetPassword();
                if(password.Length <= 0)
                {
                    Console.WriteLine("Das Passwort darf nicht leer sein!");
                    return null;
                }
                return database.login(username, password);
            }

            void register()
            {
                Console.WriteLine("Geben Sie einen Username ein!");
                var username = Console.ReadLine();
                if(username.Length <= 0)
                {
                    Console.WriteLine("Ihr Username ist zu kurz!");
                    return;
                }

                var duplicateUsers = database.getScoreboard().Where((user) => user.Username == username);
                if(duplicateUsers.Count() > 0)
                {
                    Console.WriteLine("Der Username existiert bereits! Wählen Sie einen anderen!");
                    return;
                }

                Console.WriteLine("Geben Sie ein Passwort ein!");
                var password = GetPassword();
                if (password.Length < minimumPasswordLength)
                {
                    Console.WriteLine();
                    Console.WriteLine("Das Passwort ist zu klein! Wählen Sie ein größeres Passwort!");
                    return;
                }
                
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User(username);

                database.register(user, passwordHash);

                //Deckerstellung
                database.addCardToStack(user.Username, allMonsters[0]);
                database.addCardToStack(user.Username, allMonsters[1]);
                database.addCardToStack(user.Username, allMonsters[2]);
                var random = new Random();
                int index = random.Next(allSpells.Length);
                database.addCardToStack(user.Username, allSpells[index]);
            }
            
            void shop()
            {
                Console.WriteLine("Willkommen im Shop! Hier können Sie sich Packages mit 5 Karten für 5 Münzen kaufen.");
                Console.WriteLine("Wollen Sie ein Package kaufen? Falls ja, antworten Sie mit 'y'!");

                input = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (input == 'y')
                {
                    Console.WriteLine();
                    user.Coins -= packagePrice;
                    database.updateUser(user);

                    var cards = new List<ICard>(allMonsters);
                    cards.AddRange(allSpells);
                    var random = new Random();
                    int getRange = 5;
                    var shuffledCards = cards.OrderBy((card) => random.Next()).Take(getRange).ToList();
                    foreach (var card in shuffledCards)
                    {
                        Console.WriteLine($"Karte: {card.Name}");
                    }
                    Console.WriteLine();

                    var userCardNames = database.getAllUserCards(user).Select((card) => card.Name);
                    shuffledCards.Where((card) => userCardNames.Contains(card.Name)).ToList().ForEach((card) =>
                    {
                        Console.WriteLine($"{card.Name} erhält ein Upgrade!");
                        database.upgradeCardInStack(user.Username, card);
                    }); //duplicates
                    shuffledCards.Where((card) => !userCardNames.Contains(card.Name)).ToList().ForEach((card) =>
                    {
                        Console.WriteLine($"{card.Name} wird in den Stack hinzugefügt!");
                        database.addCardToStack(user.Username, card);
                    }); //uniques
                }
            }

            bool withdrawTrade()
            {
                Console.WriteLine();
                Console.WriteLine("Wollen Sie Ihren Trade zurückziehen? Falls ja, antworten Sie mit 'y'!");


                var input = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (input == 'y')
                {
                    return database.removeTrade(user.Username);
                }
                return false;
            }

            bool trading()
            {
                for (int i = 0; i < user.Stack.Count(); i++)
                {
                    Console.WriteLine($"{i} {user.Stack[i].Name}");
                }
                Console.WriteLine();
                Console.WriteLine("Wählen Sie die zu tauschende Karte aus!");
                var input = Console.ReadLine();
                if (Int32.TryParse(input, out int number))
                {
                    if (0 <= number && number < user.Stack.Count())
                    {
                        var card = user.Stack[number];
                        Console.WriteLine();
                        Console.WriteLine("Wählen Sie den Typ der gewünschten Karte aus!");
                        Console.WriteLine("'m' für Monsterkarten");
                        Console.WriteLine("'s' für Spellkarten");
                        var typeInput = Console.ReadKey().KeyChar;
                        var requestedType = "";
                        if (typeInput == 'm')
                        {
                            requestedType = "monster";
                        }
                        else if (typeInput == 's')
                        {
                            requestedType = "spell";
                        }
                        else
                        {
                            Console.WriteLine("Kein gültiger Typ angegeben!");
                            return false;
                        }

                        Console.WriteLine();
                        Console.WriteLine("Wählen Sie den Damage der gewünschten Karte aus!");
                        Console.WriteLine("Wenn der Damage irrelevant ist, geben Sie '0' ein!");
                        var damageInput = Console.ReadLine();
                        if (!Int32.TryParse(damageInput, out int requestedDamage))
                        {
                            Console.WriteLine("Sie müssen eine Zahl eingeben!");
                            return false;
                        }

                        Console.WriteLine();
                        Console.WriteLine("Wählen Sie das Element der gewünschten Karte aus!");
                        Console.WriteLine("'w' für Wasser");
                        Console.WriteLine("'f' für Feuer");
                        Console.WriteLine("'n' für Normal");
                        Console.WriteLine("Wenn das Element irrelevant ist, geben Sie 'x' ein!");
                        var elementInput = Console.ReadKey().KeyChar;
                        ICard.Elements? requestedElement = null;
                        switch (elementInput)
                        {
                            case 'w':
                                requestedElement = ICard.Elements.Water;
                                break;
                            case 'f':
                                requestedElement = ICard.Elements.Fire;
                                break;
                            case 'n':
                                requestedElement = ICard.Elements.Normal;
                                break;
                            case 'x':
                                break;
                            default:
                                Console.WriteLine("Kein gültiges Element angegeben!");
                                return false;
                        }
                        database.setLockState(user, card, true);
                        return database.offerCardForTrade(user, card, requestedType, requestedDamage, requestedElement);
                    }
                    else
                    {
                        Console.WriteLine("Sie müssen eine (gültige) Zahl eingeben!");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Sie müssen eine (gültige) Zahl eingeben!");
                    return false;
                }
            }

            bool tradeOffers()
            {
                var trades = database.allTrades(user);
                for (int i = 0; i < trades.Count; i++)
                {
                    var trade = trades[i];
                    Console.WriteLine($"[{i}] {trade.Card.Name} offered by {trade.Username} | Wanted {trade.Requested_type.Name} with minimum {trade.Requested_damage} damage of {trade.Requested_element} element");
                }
                Console.WriteLine();
                Console.WriteLine("Wählen Sie das gewünschte Angebot aus!");
                Console.WriteLine("Wenn Ihnen kein Angebot gefällt, geben Sie 'x' ein!");
                if (!Int32.TryParse(Console.ReadLine(), out int tradeNumber))
                {
                    Console.WriteLine("Es wurde keine Zahl ausgewählt!");
                    return false;
                }
                if (tradeNumber < 0 || tradeNumber >= trades.Count())
                {
                    Console.WriteLine("Es wurde keine (gültige) Zahl ausgewählt!");
                    return false;
                }

                var activeTrade = trades[tradeNumber];

                var possibleCards = user.Stack.Where((card) => { 
                    return card.GetType() == activeTrade.Requested_type; })
                    .Where((card) => { 
                        return card.Damage >= activeTrade.Requested_damage; })
                    .Where((card) =>
                    {
                        if (activeTrade.Requested_element == ICard.Elements.Undefined)
                            return true;
                        return card.Element == activeTrade.Requested_element;
                    }).ToArray();

                if (possibleCards.Length == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sie haben keine Karte die den Kriterien entspricht!");
                    return false;
                }

                Console.WriteLine();
                Console.WriteLine("Wählen Sie eine der Karten aus, die sie tauschen wollen!");
                Console.WriteLine("Wenn Sie keine Karte abgeben wollen, geben Sie 'x' ein!");

                for (int i = 0; i < possibleCards.Length; i++)
                {
                    var card = possibleCards[i];
                    Console.WriteLine($"[{i}] {card.Name}");
                }

                if (!Int32.TryParse(Console.ReadLine(), out int swapNumber))
                {
                    Console.WriteLine("Es wurde keine Zahl ausgewählt!");
                    return false;
                }
                if (swapNumber < 0 || swapNumber >= possibleCards.Count())
                {
                    Console.WriteLine("Es wurde keine (gültige) Zahl ausgewählt!");
                    return false;
                }

                var swapCard = possibleCards[swapNumber];

                database.removeTrade(activeTrade.Username);

                if (database.isCardInUserStack(activeTrade.Username, swapCard))
                    database.upgradeCardInStack(activeTrade.Username, swapCard);
                else
                    database.addCardToStack(activeTrade.Username, swapCard);
                

                if(database.isCardInUserStack(user.Username, activeTrade.Card))
                    database.upgradeCardInStack(user.Username, activeTrade.Card);
                else
                    database.addCardToStack(user.Username, activeTrade.Card);

                database.removeCardFromStack(activeTrade.Username, activeTrade.Card);
                database.removeCardFromStack(user.Username, swapCard);
                return true;
            }

            void profile()
            {
                Console.WriteLine("Hier sehen Sie Ihre aktuellen Daten!");
                Console.WriteLine();
                Console.WriteLine($"Username: {user.Username}");
                Console.WriteLine($"Coins: {user.Coins}");
                Console.WriteLine($"ELO-Score: {user.ELO}");
                Console.WriteLine($"Gespielte Battles: {user.GamesPlayed}");

                Console.WriteLine();
                Console.WriteLine("Wollen sie ihr Passwort ändern? Wenn ja drücken sie 'y'");
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (input == 'y')
                {
                    Console.WriteLine();
                    Console.WriteLine("Geben sie jetzt ihr neues Passwort sein.");
                    var newPassword = GetPassword();
                    if(newPassword.Length < minimumPasswordLength)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Das neue Passwort muss mindestens 4 Zeichen haben!");
                        return;
                    }

                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    database.changePassword(user, passwordHash);
                    Console.WriteLine();
                    Console.WriteLine("Passwort wurde erfolgreich geändert!");
                }
            }

            void printELO(List<User> users)
            {
                int place = 1;
                Console.WriteLine("Das aktuelle Scoreboard!");
                foreach (var user in users)
                {
                    Console.WriteLine($"{place}. Username: {user.Username}     ELO-Score: {user.ELO}     Gespielte Battles: {user.GamesPlayed}");
                    place++;
                }
            }
        }
        public static string GetPassword()
        {
            var pwd = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd = pwd.Remove(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }
}
