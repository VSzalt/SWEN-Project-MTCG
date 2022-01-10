using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG_SWEN1_WS2021
{
    public class Database
    {
        public string DatabaseName = "mtcg";
        public string Password = "12dreivihr5";
        public NpgsqlConnection connection;

        public Database()
        {
            connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=12dreivihr5;Database=mtcg");
            connection.Open();
        }

        public User login(string username, string password)
        {
            var command = new NpgsqlCommand("SELECT * FROM profiles WHERE username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            usernameParameter.Value = username;
            User user = null;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var usernameResult = reader.GetString(0);
                    var passwordResult = reader.GetString(1);
                    var coinsResult = reader.GetInt32(2);
                    var eloResult = reader.GetInt32(3);
                    var gamesPlayedResult = reader.GetInt32(4);

                    if (BCrypt.Net.BCrypt.Verify(password, passwordResult))
                    {
                        user = new User(usernameResult, coinsResult, eloResult, gamesPlayedResult);
                    }
                }
            }
            return user;
        }
        
        public bool register(User user, string password)
        {
            var command = new NpgsqlCommand("INSERT INTO profiles (username,password,coins,elo,gamesplayed) VALUES (@username,@password,@coins,@elo,@gamesplayed)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var passwordParameter = command.Parameters.Add("password", NpgsqlTypes.NpgsqlDbType.Varchar);
            var coinsParameter = command.Parameters.Add("coins", NpgsqlTypes.NpgsqlDbType.Integer);
            var eloParameter = command.Parameters.Add("elo", NpgsqlTypes.NpgsqlDbType.Integer);
            var gamesplayed = command.Parameters.Add("gamesplayed", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = user.Username;
            passwordParameter.Value = password;
            coinsParameter.Value = user.Coins;
            eloParameter.Value = user.ELO;
            gamesplayed.Value = user.GamesPlayed;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public bool changePassword(User user, string password)
        {
            var command = new NpgsqlCommand("UPDATE profiles SET password = @password WHERE username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var passwordParameter = command.Parameters.Add("password", NpgsqlTypes.NpgsqlDbType.Varchar);

            command.Prepare();

            usernameParameter.Value = user.Username;
            passwordParameter.Value = password;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public bool updateUser(User user)
        {
            var command = new NpgsqlCommand("UPDATE profiles SET coins = @coins, elo = @elo, gamesplayed = @gamesplayed WHERE username = @username", connection);
            var coinsParameter = command.Parameters.Add("coins", NpgsqlTypes.NpgsqlDbType.Integer);
            var eloParameter = command.Parameters.Add("elo", NpgsqlTypes.NpgsqlDbType.Integer);
            var gamesPlayedParameter = command.Parameters.Add("gamesplayed", NpgsqlTypes.NpgsqlDbType.Integer);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            coinsParameter.Value = user.Coins;
            eloParameter.Value = user.ELO;
            gamesPlayedParameter.Value = user.GamesPlayed;
            usernameParameter.Value = user.Username;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public bool offerCardForTrade(User user, ICard card, string requestedType, int requestedDamage, ICard.Elements? requestedElement)
        {
            var command = new NpgsqlCommand("INSERT INTO trades (username,monster_id,spell_id,requested_type,requested_damage,requested_element) VALUES (@username,@monster_id,@spell_id,@requested_type,@requested_damage,@requested_element)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var requested_typeParameter = command.Parameters.Add("requested_type", NpgsqlTypes.NpgsqlDbType.Unknown);
            var requested_damageParameter = command.Parameters.Add("requested_damage", NpgsqlTypes.NpgsqlDbType.Integer);
            var requested_elementParameter = command.Parameters.Add("requested_element", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = user.Username;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if (card is SpellCard)
                spell_idParameter.Value = card.Id;
            requested_typeParameter.Value = requestedType;
            requested_damageParameter.Value = requestedDamage;
            requested_elementParameter.Value = DBNull.Value;
            if (requestedElement is not null)
                requested_elementParameter.Value = (int)requestedElement;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public bool setLockState(User user, ICard card, bool lockState)//lock = true, unlock = false
        {
            var command = new NpgsqlCommand("UPDATE stacks SET locked = @lockState WHERE username = @username AND (monster_id = @monster_id OR spell_id = @spell_id)", connection);
            var lockStateParameter = command.Parameters.Add("lockState", NpgsqlTypes.NpgsqlDbType.Boolean);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);           
            command.Prepare();

            lockStateParameter.Value = lockState;
            usernameParameter.Value = user.Username;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if (card is SpellCard)
                spell_idParameter.Value = card.Id;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public bool isUserTrading(User user)
        {
            var command = new NpgsqlCommand("SELECT COUNT(1) FROM trades WHERE username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            usernameParameter.Value = user.Username;

            var count = -1;
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    count = result.GetInt32(0);                    
                }
            }                
            return count > 0;            
        }

        public bool isCardInUserStack(string username, ICard card)
        {
            var command = new NpgsqlCommand("SELECT COUNT(1) FROM stacks WHERE username = @username AND (monster_id = @monster_id OR spell_id = @spell_id)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = username;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if (card is SpellCard)
                spell_idParameter.Value = card.Id;

            var count = -1;
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    count = result.GetInt32(0);
                }
            }
            return count > 0;
        }

        public ICard getCardFromActiveTrade(User user)
        {
            var command = new NpgsqlCommand("SELECT monster_id, spell_id FROM trades WHERE username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            usernameParameter.Value = user.Username;

            ICard card = new SpellCard(-1);//Placeholder            
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {                    
                    if (result.IsDBNull(0))
                    {                        
                        card = new SpellCard(result.GetInt32(1));
                    }
                    if (result.IsDBNull(1))
                    {
                        card = new MonsterCard(result.GetInt32(0));
                    }                    
                }
            }
            
            return card;
        }

        public bool removeTrade(string username)
        {
            var command = new NpgsqlCommand("DELETE FROM trades WHERE username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            usernameParameter.Value = username;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public List<TradingOffer> allTrades(User user)
        {
            var command = new NpgsqlCommand("SELECT username,monster_id,spell_id,requested_damage,requested_element, requested_type FROM trades WHERE username != @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            command.Prepare();

            usernameParameter.Value = user.Username;

            var list = new List<TradingOffer>();            
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var tradingOffer = new TradingOffer(result.GetString(0));
                    tradingOffer.Requested_element = ICard.Elements.Undefined;
                    
                    ICard card = new SpellCard(-1);//Placeholder                    
                    if (result.IsDBNull(1))
                    {
                        card = new SpellCard(result.GetInt32(2));
                    }
                    if (result.IsDBNull(2))
                    {
                        card = new MonsterCard(result.GetInt32(1));
                    }
                    tradingOffer.Card = card;
                    if (!result.IsDBNull(3))
                    {
                        tradingOffer.Requested_damage = result.GetInt32(3);
                    }
                    if (!result.IsDBNull(4))
                    {                        
                        tradingOffer.Requested_element = (ICard.Elements)result.GetInt32(4);
                    }
                    //(ICard.Elements)Enum.Parse(typeof(ICard.Elements), result.GetInt32(4).ToString());                    
                    string requestedType = result.GetString(5);
                    if (requestedType.Equals("monster"))
                        tradingOffer.Requested_type = typeof(MonsterCard);
                    if (requestedType.Equals("spell"))
                        tradingOffer.Requested_type = typeof(SpellCard);

                    list.Add(tradingOffer);
                }
            }

            for (int i = 0; i < list.Count(); i++)
            {
                if(list[i].Card is MonsterCard)
                {
                    list[i].Card = getMonsterCardFromUserById(list[i].Username, list[i].Card.Id);
                }
                if(list[i].Card is SpellCard)
                {
                    list[i].Card = getSpellCardFromUserById(list[i].Username, list[i].Card.Id);
                }
            }

            return list;
        }

        public bool removeCardFromStack(string username, ICard card)
        {
            var command = new NpgsqlCommand("DELETE FROM stacks WHERE username = @username AND (monster_id = @monster_id OR spell_id = @spell_id)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = username;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if (card is SpellCard)
                spell_idParameter.Value = card.Id;

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public List<User> getScoreboard()
        {
            var command = new NpgsqlCommand("SELECT username,elo,gamesplayed FROM profiles ORDER BY elo DESC", connection);
            command.Prepare();

            var list = new List<User>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var username = result.GetString(0);                  
                    var elo = result.GetInt32(1);
                    var gamesplayed = result.GetInt32(2);

                    var user = new User(username);
                    user.ELO = elo;
                    user.GamesPlayed = gamesplayed;
                    list.Add(user);
                }
            }
            return list;
        }
        public bool addCardToStack(string username, ICard card)
        {
            var command = new NpgsqlCommand("INSERT INTO stacks (username,monster_id,spell_id) VALUES (@username,@monster_id,@spell_id)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = username;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if (card is SpellCard)
                spell_idParameter.Value = card.Id;
            var rowsAffected = command.ExecuteNonQuery();

            return rowsAffected == 1;
        }
        public bool upgradeCardInStack(string username, ICard card)
        {
            var command = new NpgsqlCommand("UPDATE stacks SET upgrade = upgrade + @upgrade WHERE username = @username AND (monster_id = @monster_id OR spell_id = @spell_id)", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var monster_idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var spell_idParameter = command.Parameters.Add("spell_id", NpgsqlTypes.NpgsqlDbType.Integer);
            var upgradeParameter = command.Parameters.Add("upgrade", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();


            usernameParameter.Value = username;
            upgradeParameter.Value = 1 + card.Upgrade;
            monster_idParameter.Value = DBNull.Value;
            spell_idParameter.Value = DBNull.Value;
            if (card is MonsterCard)
                monster_idParameter.Value = card.Id;
            if(card is SpellCard)
                spell_idParameter.Value = card.Id;         
            
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        public MonsterCard getMonsterCardFromUserById(string username, int monster_id)
        {
            var command = new NpgsqlCommand("SELECT id,name,element,damage,species,upgrade FROM stacks JOIN monster ON stacks.monster_id = monster.id WHERE stacks.username = @username AND stacks.monster_id = @monster_id", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = username;
            idParameter.Value = monster_id;

            var monster = new MonsterCard(-1);
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);
                    var species = result.GetInt32(4);
                    var upgrade = result.GetInt32(5);

                    monster = new MonsterCard(id, (ICard.Elements)element, name, damage, (MonsterCard.Species)species, upgrade);
                }
            }

            return monster;
        }

        public SpellCard getSpellCardFromUserById(string username, int spell_id)
        {
            var command = new NpgsqlCommand("SELECT id,name,element,damage,upgrade FROM stacks JOIN spell ON stacks.spell_id = spell.id WHERE stacks.username = @username", connection);
            var usernameParameter = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var idParameter = command.Parameters.Add("monster_id", NpgsqlTypes.NpgsqlDbType.Integer);
            command.Prepare();

            usernameParameter.Value = username;
            idParameter.Value = spell_id;

            var spell = new SpellCard(-1);
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);
                    var upgrade = result.GetInt32(4);

                    spell = new SpellCard(id, (ICard.Elements)element, name, damage, upgrade);
                }
            }

            return spell;
        }

        public List<ICard> getAllUserCards(User user)
        {
            var monsterCommand = new NpgsqlCommand("SELECT id,name,element,damage,species,upgrade FROM stacks JOIN monster ON stacks.monster_id = monster.id WHERE stacks.username = @username", connection);
            var spellCommand = new NpgsqlCommand("SELECT id,name,element,damage,upgrade FROM stacks JOIN spell ON stacks.spell_id = spell.id WHERE stacks.username = @username", connection);
            var monsterUsernameParameter = monsterCommand.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            var spellUsernameParameter = spellCommand.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
            monsterCommand.Prepare();
            spellCommand.Prepare();

            monsterUsernameParameter.Value = user.Username;
            spellUsernameParameter.Value = user.Username;
            var list = new List<ICard>();
            using (var result = monsterCommand.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);
                    var species = result.GetInt32(4);
                    var upgrade = result.GetInt32(5);

                    var monster = new MonsterCard(id, (ICard.Elements)element, name, damage, (MonsterCard.Species)species, upgrade);
                    list.Add(monster);
                }
            }
            using (var result = spellCommand.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);
                    var upgrade = result.GetInt32(4);

                    var spell = new SpellCard(id, (ICard.Elements)element, name, damage, upgrade);
                    list.Add(spell);
                }
            }
            return list;
        }
        public MonsterCard[] getAllMonsters()
        { 
            var command = new NpgsqlCommand("SELECT * FROM monster", connection);
            var list = new List<MonsterCard>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);
                    var species = result.GetInt32(4);

                    var monster = new MonsterCard(id, (ICard.Elements)element, name, damage, (MonsterCard.Species)species, 0);
                    list.Add(monster);
                }
            }
            return list.ToArray();
        }

        public SpellCard[] getAllSpells()
        {
            var command = new NpgsqlCommand("SELECT * FROM spell", connection);
            var list = new List<SpellCard>();
            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var id = result.GetInt32(0);
                    var name = result.GetString(1);
                    var element = result.GetInt32(2);
                    var damage = result.GetInt32(3);

                    var spell = new SpellCard(id, (ICard.Elements)element, name, damage, 0);
                    list.Add(spell);
                }
            }

            return list.ToArray();
        }

    }
}
