using CampusParks.Utils;
using CitiesHarmony.API;
using ICities;

namespace CampusParks
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Campus Parks Renewed";

        public string Description => "Allows to place parks on pedestrian paths in campuses, airports and industry zones";

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }
    }
}