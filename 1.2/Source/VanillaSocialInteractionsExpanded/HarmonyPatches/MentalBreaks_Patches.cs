﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(MentalBreakWorker_RunWild), "TryStart")]
	public class MentalBreakWorker_RunWild_Patch
	{
		private static void Postfix(Pawn pawn, string reason, bool causedByMood)
        {
			if (Rand.Chance(0.1f))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RanWild, pawn);
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_Slaughterer), "Notify_SlaughteredAnimal")]
	public class Notify_SlaughteredAnimal_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			if (Rand.Chance(0.1f))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_SlaughteredAnimalInRage, ___pawn);
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_Jailbreaker), "Notify_InducedPrisonerToEscape")]
	public class Notify_InducedPrisonerToEscape_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			if (Rand.Chance(0.1f))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InducedPrisonerToEscape, ___pawn);
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_SocialFighting), "PostEnd")]
	public class PostEnd_Patch
	{
		private static void Postfix(MentalState_SocialFighting __instance)
		{
			if (Rand.Chance(0.1f))
            {
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadSocialFight, __instance.pawn, __instance.otherPawn);
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadSocialFight, __instance.otherPawn, __instance.pawn);
			}
		}
	}

	[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
	public class TryStartMentalState_Patch
	{
		private static void Postfix(MentalStateHandler __instance, Pawn ___pawn, bool __result, MentalStateDef stateDef, string reason = null, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false)
		{
			if (__result)
            {
				if (stateDef == MentalStateDefOf.Wander_OwnRoom)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_HideInRoom, ___pawn);
					}
				}
				else if (stateDef == MentalStateDefOf.Wander_Sad)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WanderedInSaddness, ___pawn);
					}
				}
				else if (stateDef == VSIE_DefOf.SadisticRage)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentIntoSadisticRage, ___pawn);
					}
				}
				else if (stateDef == VSIE_DefOf.Tantrum || stateDef == VSIE_DefOf.BedroomTantrum || stateDef == VSIE_DefOf.TargetedTantrum)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ThrewTantrum, ___pawn);
					}
				}
				else if (stateDef == MentalStateDefOf.Berserk)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentBerserk, ___pawn);
					}
				}
				else if (stateDef == VSIE_DefOf.FireStartingSpree)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentOnFireStartingSpree, ___pawn);
					}
				}
				else if (stateDef == VSIE_DefOf.MurderousRage)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentOnMurderousRage, ___pawn);
					}
				}
			}
		}
	}
}
