using System.Collections.Generic;
using Features.Card.Data;

namespace Features.Shop.Data
{
    public class ShopCardPackSlot
    {
        public string Name;
        public int Price;
        public bool IsSold;
        public List<CardData> Candidates;
        public int TotalOptions;
        public int PickCount;
        public string RarityFilter;
        public string Desc;
        public string Address;
    }
}