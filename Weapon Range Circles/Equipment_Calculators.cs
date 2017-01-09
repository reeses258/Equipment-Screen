using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Linq;


namespace WRS
{
    public class Equipment_Ranged_Calculators
    {
        public static float Damage( ThingDef d )
        {
            return ( d.Verbs[ 0 ].projectileDef == null ) ? 0 : d.Verbs[ 0 ].projectileDef.projectile.damageAmountBase;
        }

        public static float Warmup( ThingDef d )
        {
            return d.Verbs[ 0 ].warmupTime;
        }

        public static float Cooldown( Thing d )
        {
            return d.GetStatValue( StatDefOf.RangedWeapon_Cooldown, true );
        }

        public static float BurstShotCount( ThingDef d )
        {
            return d.Verbs[ 0 ].burstShotCount;
        }

        public static float DPSRaw( Thing d )
        {
            float num = BurstShotCount( d.def );
            float num2 = Warmup( d.def ) + Cooldown( d );
            num2 += ( float ) ( num - 1 ) * ( ( float ) d.def.Verbs[ 0 ].ticksBetweenBurstShots / 60f );
            return ( Damage( d.def ) * num ) / num2;
        }

        public static float AccuracyTouch( Thing d )
        {
            return d.GetStatValue( StatDefOf.AccuracyTouch, true );
        }

        public static float AccuracyShort( Thing d )
        {
            return d.GetStatValue( StatDefOf.AccuracyShort, true );
        }

        public static float AccuracyMedium( Thing d )
        {
            return d.GetStatValue( StatDefOf.AccuracyMedium, true );
        }

        public static float AccuracyLong( Thing d )
        {
            return d.GetStatValue( StatDefOf.AccuracyLong, true );
        }

        public static float AverageDPS( Thing d )
        {
            float num = 0f;
            num += AverageShortDPS( d );
            num += AverageMediumDPS( d );
            num += AverageLongDPS( d );
            return num / 3;
        }

        public static float AverageShortDPS( Thing d )
        {
            return DPSRaw( d ) * AccuracyShort( d );
        }

        public static float AverageMediumDPS( Thing d )
        {
            return DPSRaw( d ) * AccuracyMedium( d );
        }

        public static float AverageLongDPS( Thing d )
        {
            return DPSRaw( d ) * AccuracyLong( d );
        }
    }
    public class Equipment_Melee_Calculators
    {
        public static float Damage( Pawn p, Thing d )
        {
            //return v.verbProps.AdjustedMeleeDamageAmount( v, p, d );
            //(Verb)Activator.CreateInstance(verbProperties.verbClass);

            if( p == null || d == null)
            {
                Log.Message( " Pawn or thing null" );
                return 0f;
            }

            //Verb v = ( Verb ) Activator.CreateInstance( d.def.Verbs[ 0 ].verbClass );
            //v.loadID = Find.World.uniqueIDsManager.GetNextVerbID();
            //v.caster = p;
            //v.verbProps = d.def.Verbs[0];
            //float num = d.GetStatValue( StatDefOf.MeleeWeapon_DamageAmount, true );
            //num *=  PawnCapacityUtility.CalculatePartEfficiency( v.ownerHediffComp.Pawn.health.hediffSet, v.ownerHediffComp.parent.Part, false );
            ////num *= v.GetDamageFactorFor( p );
            //Log.Message( "Verb properties: " + v.GetDamageFactorFor( p ) );
            //return num;
            //if( v != null )
            // return v.verbProps.AdjustedMeleeDamageAmount( v, p, d );
            //Log.Message( " Damage calculator no verb " + v.ToString());
            //return 0f;
            
            //RimWorld.StatExtension.GetStatValue( d, StatDef)
            return d.GetStatValue( StatDefOf.MeleeWeapon_DamageAmount, true);
        }

        public static float Cooldown( Thing d )
        {
            return d.GetStatValue( StatDefOf.MeleeWeapon_Cooldown, true );
        }

        public static float Warmup( ThingDef d )
        {
            ThingDef thingDef = d as ThingDef;
            if( thingDef != null )
            {
                return thingDef.Verbs.Average( ( VerbProperties v ) => v.warmupTime );
            }
      //      HediffDef hediffDef = d as HediffDef;
          //  if( hediffDef != null )
            {
         //       return hediffDef.CompProps<HediffCompProperties_VerbGiver>().verbs[ 0 ].warmupTime;
            }
            return -1f;
        }
        public static float DPS( Pawn p, Thing d )
        {
            //return 1;
            return Damage( p, d ) / ( Warmup( d.def ) + Cooldown( d ) );
        }
    }
}

