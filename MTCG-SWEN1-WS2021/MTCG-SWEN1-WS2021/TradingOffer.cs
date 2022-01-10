using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_SWEN1_WS2021
{
    public class TradingOffer
    {
        public TradingOffer(string username, ICard card, int requested_damage, ICard.Elements requested_element, Type requested_type)
        {
            Username = username;
            Card = card;
            Requested_damage = requested_damage;
            Requested_element = requested_element;
            Requested_type = requested_type;
        }

        public TradingOffer(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
        public ICard Card { get; set; }
        public int Requested_damage { get; set; }
        public ICard.Elements Requested_element { get; set; }
        public Type Requested_type { get; set; }
    }
}
