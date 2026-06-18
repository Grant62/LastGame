using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Features.Card.Utility
{
    public class CardSpriteCache : ICardSpriteCache
    {
        private readonly Dictionary<string, Sprite> mCache = new();

        public Sprite GetSprite(string iconAddress)
        {
            if (string.IsNullOrEmpty(iconAddress))
                return null;

            if (mCache.TryGetValue(iconAddress, out Sprite cached))
                return cached;

            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(iconAddress);
            handle.WaitForCompletion();

            Assert.IsNotNull(handle.Result, $"Addressables load failed: [{iconAddress}]");
            mCache[iconAddress] = handle.Result;
            return handle.Result;
        }
    }
}