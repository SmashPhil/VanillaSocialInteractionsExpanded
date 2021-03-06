﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
	public class TryInteractWith_Patch
	{
		private static void Postfix(bool __result, Pawn ___pawn,  Pawn recipient, InteractionDef intDef)
		{
			if (__result)
            {
				if (intDef == InteractionDefOf.Insult)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InsultedMe, recipient, ___pawn);
					}
				}
				else if (intDef == InteractionDefOf.Chitchat || intDef == InteractionDefOf.DeepTalk)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, recipient, ___pawn);
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, ___pawn, recipient);
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "SuccessChance")]
	public class SuccessChance_Patch
	{
		private static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (initiator.InspirationDef == VSIE_DefOf.VSIE_Flirting_Frenzy)
            {
				__result *= 2f;
            }
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "Interacted")]
	public class InteractionWorker_RecruitAttempt_Interacted_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo mindStateInfo = AccessTools.Field(typeof(Pawn), "mindState");
			FieldInfo inspirationHandlerInfo = AccessTools.Field(typeof(Pawn_MindState), "inspirationHandler");
			FieldInfo inspired_TamingInfo = AccessTools.Field(typeof(InspirationDefOf), "Inspired_Taming");

			var codes = instructions.ToList();
			bool found = false;
			for (var i = 0; i < codes.Count; i++)
			{
				if (!found && codes[i].OperandIs(mindStateInfo) && codes[i + 1].OperandIs(inspirationHandlerInfo) && codes[i + 2].OperandIs(inspired_TamingInfo))
				{
					found = true;
					Log.Message($"{i} - codes[i]: {codes[i]}, codes[i + 1]: {codes[i + 1]}, codes[i + 2]: {codes[i + 2]}, codes[i + 3]: {codes[i + 3]}");
					yield return new CodeInstruction(OpCodes.Ldarg_1, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractionWorker_RecruitAttempt_Interacted_Patch), "Notify_Progress", null, null));
					yield return codes[i];
				}
				else
				{
					yield return codes[i];
				}
			}
			yield break;
		}

		public static void Notify_Progress(Pawn pawn)
		{
			if (pawn.InspirationDef == VSIE_DefOf.Inspired_Taming)
			{
				VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
			}
		}
	}


	[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new Type[] 
	{
		typeof(Pawn), typeof(Pawn), typeof(float), typeof(string), typeof(string), typeof(bool), typeof(bool)
	}, new ArgumentType[] 
	{
		ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal
	})]
	public class DoRecruit_Patch
	{
		private static void Prefix(Pawn recruiter, Pawn recruitee)
        {
			if (recruitee.IsPrisoner && recruiter?.InspirationDef == VSIE_DefOf.Inspired_Recruitment)
            {
				VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(recruiter);
            }
        }
		private static void Postfix(Pawn recruiter, Pawn recruitee)
		{
			if (recruitee.def == ThingDefOf.Thrumbo)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_TamedThrumbo, recruiter);
				}
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_Breakup), "Interacted")]
	public class Interacted_Patch
	{
		private static void Postfix(Pawn initiator, Pawn recipient)
		{
			if (Rand.Chance(0.1f))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BrokeUpWithMe, recipient, initiator);
			}
		}
	}
}
