using System;
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
            yield return ParkMethod(nameof(ParkBuildingAI.CreateBuilding));
            yield return ParkMethod(nameof(ParkBuildingAI.BuildingLoaded));
            yield return ParkMethod(nameof(ParkBuildingAI.ReleaseBuilding));
            yield return ParkMethod(nameof(ParkBuildingAI.BeginRelocating));
            yield return ParkMethod(nameof(ParkBuildingAI.EndRelocating));
            yield return ParkMethod(nameof(ParkBuildingAI.ParkAreaChanged));
            yield return ParkMethod(nameof(ParkBuildingAI.CheckBuildPosition));
            yield return ParkMethod(nameof(ParkBuildingAI.CheckRoadAccess));
            yield return ParkMethod("FindRoadAccess");
            yield return ParkMethod("HandleCrime");
            yield return ParkMethod("HandleCommonConsumption");
            yield return ParkMethod("HandleDead2");
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

        private static MethodBase ParkMethod(string name)
        {
            var method = AccessTools.DeclaredMethod(typeof(ParkBuildingAI), name) ?? throw new Exception($"ParkBuildingAI does not declare or override: {name}");
            return method;
        }

        private static bool IsPark(ref DistrictPark park)
        {
            return DistrictPark.IsParkType(park.m_parkType) || DistrictPark.IsCampusType(park.m_parkType) || DistrictPark.IsAirportType(park.m_parkType) || DistrictPark.IsIndustryType(park.m_parkType);
        }
    }
}