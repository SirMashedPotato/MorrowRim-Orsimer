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

        public static bool MadeOfThing(Thing t, List<string> stuffList)
        {
            if (t.def.costList != null)
            {
                foreach(ThingDefCountClass ing in t.def.costList)
                {
                    if (stuffList.Contains(ing.thingDef.defName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool RightSkill(RecipeDef recipe, List<SkillDef> skillList)
        {
            return skillList != null && skillList.Contains(recipe.workSkill);
        }

        public static bool RequiredTrait(Pawn p, TraitDef t, bool nullIsTrue = true)
        {
            return (t == null && nullIsTrue) || (p.story.traits != null && p.story.traits.HasTrait(t));
        }

        public static bool RequiredHediff(Pawn p, HediffDef h, bool nullIsTrue = true)
        {
            return (h == null && nullIsTrue) || (p.health.hediffSet.GetFirstHediffOfDef(h) != null);
        }

        public static bool RequiredBackstory(Pawn p, Backstory b, bool nullIsTrue = true)
        {
            return (b == null && nullIsTrue)
                || (p.story.GetBackstory(BackstorySlot.Childhood) != null && p.story.GetBackstory(BackstorySlot.Childhood) == b) 
                || (p.story.GetBackstory(BackstorySlot.Adulthood) != null && p.story.GetBackstory(BackstorySlot.Adulthood) == b);
        }

        public static bool OnlyOneCheck(Pawn p, TraitDef t, HediffDef h, Backstory b)
        {
            return ((RequiredTrait(p, t, false) || RequiredHediff(p, h, false) || RequiredBackstory(p, b, false)) 
                || (RequiredTrait(p, t) && RequiredHediff(p, h) && RequiredBackstory(p, b)));
        }

        public static bool ChanceIncrease()
        {
            return Rand.Chance(ESCP_Orsimer_Mod.ESCP_Orsimer_EnableOrichalcPatchChance());
        }

        public static QualityCategory CheckQualityIncrease(Pawn worker, QualityCategory initial, Thing thing, RecipeDef recipe)
        {
            var modExt = StuffKnowledge.Get(worker.def);
            if (initial != QualityCategory.Legendary && modExt != null)
            {
                if ((modExt.allOrNothing && RequiredTrait(worker, modExt.requiredTrait) && RequiredHediff(worker, modExt.requiredHediff) && RequiredBackstory(worker, modExt.requiredBackstory)) ||
                    (!modExt.allOrNothing && OnlyOneCheck(worker, modExt.requiredTrait, modExt.requiredHediff, modExt.requiredBackstory)))
                {
                    if (RightSkill(recipe, modExt.skillList) && (MadeOfStuff(thing, modExt.stuffList) || (modExt.notJustStuff && MadeOfThing(thing, modExt.stuffList))) && ChanceIncrease())
                    {
                        if (ESCP_Orsimer_Mod.ESCP_Orsimer_Logging()) Log.Message("Initial quality of  " + thing + " = " + initial + ", improved quality = " + (initial + 1)); ;
                        return initial + 1;
                    }
                }
            }
            return initial;
        }
    }
}
