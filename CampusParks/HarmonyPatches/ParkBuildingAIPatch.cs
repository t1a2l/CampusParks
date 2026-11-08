using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace CampusParks.HarmonyPatches
{
    [HarmonyPatch]
    public static class ParkBuildingAIPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.CreateBuilding));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.BuildingLoaded));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.ReleaseBuilding));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.BeginRelocating));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.EndRelocating));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.ParkAreaChanged));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.CheckBuildPosition));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.CheckRoadAccess));
            yield return AccessTools.Method(typeof(ParkBuildingAI), "FindRoadAccess");
            yield return AccessTools.Method(typeof(ParkBuildingAI), "HandleCrime");
            yield return AccessTools.Method(typeof(ParkBuildingAI), "HandleCommonConsumption");
            yield return AccessTools.Method(typeof(ParkBuildingAI), "HandleDead2");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var isParkGetter = AccessTools.PropertyGetter(typeof(DistrictPark), nameof(DistrictPark.IsPark));
            var replacement = AccessTools.Method(typeof(ParkBuildingAIPatch), nameof(IsPark));

            foreach (var instruction in instructions)
            {
                if ((instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) && instruction.operand is MethodInfo method && method == isParkGetter)
                {
                    yield return new CodeInstruction(OpCodes.Call, replacement);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        private static bool IsPark(ref DistrictPark park)
        {
            return DistrictPark.IsParkType(park.m_parkType) || DistrictPark.IsCampusType(park.m_parkType) || DistrictPark.IsAirportType(park.m_parkType) || DistrictPark.IsIndustryType(park.m_parkType);
        }
    }
}