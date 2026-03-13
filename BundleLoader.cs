using Comfort.Common;
using EFT.AssetsManager;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace RaiRai.HiddenCaches
{
    internal static class BundleLoader
    {
        internal static AudioClip audioClip;
        internal static Material material;
        internal static ParticleSystem particleSystem;

        internal static async Task PopulateComponentsAsync()
        {
            var loadedBundle = await LoadBundle();
            audioClip = loadedBundle.Item1;
            material = loadedBundle.Item2;
            particleSystem = loadedBundle.Item3;
        }

        internal static async Task<Tuple<AudioClip, Material, ParticleSystem>> LoadBundle()
        {
            var PATH = "assets/content/location_objects/lootable/prefab/scontainer_crate.bundle";

            // Confirmed via PoolManagerClass decompile:
            //   public readonly IEasyAssets EasyAssets;   (field on PoolManagerClass)
            //   public PoolManagerClass(IEasyAssets easyAssets, ...) { this.EasyAssets = easyAssets; }
            // PoolManagerClass is the registered Singleton - IEasyAssets is just the interface it holds.
            // Retain() and GetAsset() are extension methods on IEasyAssets, defined in EFT.AssetsManager.
            var easyAssets = Singleton<PoolManagerClass>.Instance.EasyAssets;

            await easyAssets.Retain(PATH, null, null).LoadingJob;

            try
            {
                var allComponents = easyAssets.GetAsset<GameObject>(PATH).GetComponentsInChildren<Component>();

                foreach (var component in allComponents)
                {
                    if (component.name == "Flare_Smoke" && component.GetType().Name == "ParticleSystemRenderer")
                    {
                        ParticleSystemRenderer renderer = component.GetComponent<ParticleSystemRenderer>();
                        material = renderer.material;
                        continue;
                    }

                    if (component.name == "Flare_Audio" && component.GetType().Name == "AudioSource")
                    {
                        AudioSource audioSource = component.GetComponent<AudioSource>();
                        audioClip = audioSource.clip;
                        continue;
                    }

                    if (component.name == "Flare_Smoke" && component.GetType().Name == "ParticleSystem")
                    {
                        particleSystem = component.GetComponent<ParticleSystem>();
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[HiddenCaches] BundleLoader failed to extract components: {ex}");
            }

            return Tuple.Create(audioClip, material, particleSystem);
        }
    }
}
