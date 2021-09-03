﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection.Emit;
using System.Linq;

namespace MorrowRim_Orsimir
{
    public static class HarmonyUtility
    {
        /* For qulity patch */
        public static bool MadeOfStuff(Thing t, List<string> stuffList)
        {
            return t != null && t.Stuff != null && stuffList.Contains(t.Stuff.ToString());
        }

        public static bool RightSkill(RecipeDef recipe, SkillDef skill)
        {
            return skill != null && recipe.workSkill == skill;
        }

        public static bool ChanceIncrease()
        {
            bool chance = Rand.Chance(ESCP_Orsimer_Mod.ESCP_Orsimer_EnableOrichalcPatchChance());
            return chance;
        }

        public static QualityCategory CheckQualityIncrease(Pawn worker, QualityCategory initial, Thing thing, RecipeDef recipe)
        {
            var modExt = StuffKnowledge.Get(worker.def);
            if (initial != QualityCategory.Legendary && modExt != null)
            {

                if (RightSkill(recipe, modExt.skill) && MadeOfStuff(thing, modExt.stuffList) && ChanceIncrease())
                {
                    if (ESCP_Orsimer_Mod.ESCP_Orsimer_Logging()) Log.Message("Initial quality of  " + thing + " = " + initial + ", improved quality = " + (initial + 1)); ;
                    return initial + 1;
                }
            }
            return initial;
        }

        /* for taming patch */
        /*
        public static bool IsOrsimerTamable(Pawn p)
        {
            return p != null && (p.def == ThingDef.Named("ESCP_Echatere") || p.def == ThingDef.Named("ESCP_Welwa"));
        }
        */
    }
}
