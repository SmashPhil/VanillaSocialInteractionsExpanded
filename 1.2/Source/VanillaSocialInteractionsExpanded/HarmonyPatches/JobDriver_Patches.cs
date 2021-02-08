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
	[HarmonyPatch(typeof(JobDriver_HaulCorpseToPublicPlace), "MakeNewToils")]
	public class JobDriver_HaulCorpseToPublicPlace_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			List<Toil> toils = __result.ToList();
			var toil = new Toil();
			toil.initAction = delegate ()
			{
				var actor = toil.actor;
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ExposedCorpseOfMyFriend, actor, (actor.CurJob.GetTarget(TargetIndex.A).Thing as Corpse).InnerPawn);
			};
			toils.Add(toil);
			__result = toils;
		}
	}

	[HarmonyPatch(typeof(JobDriver_Ingest), "MakeNewToils")]
	public class JobDriver_Ingest_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			List<Toil> toils = __result.ToList();
			var toil = new Toil();
			toil.initAction = delegate ()
			{
				var actor = toil.actor;
				if (actor.CurJob.overeat)
                {
					if (actor.CurJob.GetTarget(TargetIndex.A).Thing.def.IsDrug)
                    {
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BingedDrug, actor);
					}
					else
                    {
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BingedFood, actor);
                    }
				}
			};
			toils.Add(toil);
			__result = toils;
		}
	}
}