using SPT.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Reflection;

namespace RaiRai.HiddenCaches
{
    internal class RaidEndPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.Player).GetMethod("OnDestroy",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        [PatchPrefix]
        private static void Cleanup(EFT.Player __instance)
        {
            if (Singleton<GameWorld>.Instance?.MainPlayer?.Id == __instance.Id)
            {
                BundleLoader.material = null;
                BundleLoader.audioClip = null;
                BundleLoader.particleSystem = null;

                CachePatch.hiddenCacheList = null;
            }
        }
    }
}
