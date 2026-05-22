#if ENABLE_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace QFramework
{
    public class AddressableAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new AddressableAudioLoader();
        }

        public class AddressableAudioLoader : IAudioLoader
        {
            public AudioClip Clip { get; private set; }

            public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
            {
                Clip = Addressables.LoadAssetAsync<AudioClip>(audioSearchKeys.AssetName).WaitForCompletion();
                return Clip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
            {
                Addressables.LoadAssetAsync<AudioClip>(audioSearchKeys.AssetName).Completed += handle =>
                {
                    Clip = handle.Result;
                    onLoad?.Invoke(handle.Result != null, handle.Result);
                };
            }

            public void Unload()
            {
                if (Clip != null)
                {
                    Addressables.Release(Clip);
                    Clip = null;
                }
            }
        }
    }
}
#endif