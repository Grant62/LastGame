using System.Collections.Generic;
using Features.Card.Data;
using QFramework;

namespace Features.Shop.System
{
    public interface IShopSystem : ISystem
    {
        void GenerateShop();
        List<CardData> GenerateCandidates(int slotIndex);
        void MarkSold(int slotIndex);
    }
}