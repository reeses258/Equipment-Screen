using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HugsLib;
using System;

namespace Weapon_Range_Circles
{
    //class HitChanceViewer : ModBase
    //{
    // //   static bool displayActive = false;
    //    static HitChanceDialog chanceDialog = null;

    //    public override string ModIdentifier
    //    {
    //        get
    //        {
    //            return "Weapons_HitChanceViewer";
    //        }
    //    }

    //    public override void Update()
    //    {
    //        if( Current.Game == null )
    //            return;

    //        List<object> selected = Find.Selector.SelectedObjects;
    //        IntVec3 mouseCell = UI.MouseCell();
    //        Rect windowLoc = new Rect( mouseCell.x, mouseCell.y, 50, 50 );
    //        if( selected.Count == 1 )
    //        {
    //            if( selected[ 0 ].GetType() == typeof(Pawn) )
    //            {
    //                Pawn pwn = ( Pawn ) selected[ 0 ];
    //                Vector3 pwnLoc = new Vector3( pwn.Position.x, pwn.Position.y, pwn.Position.z );
    //                Vector3 mouseLoc = new Vector3( mouseCell.x, mouseCell.y, mouseCell.z );
    //                GenDraw.DrawLineBetween( pwnLoc, mouseLoc );
    //                //chanceDialog.Close();

    //                if( chanceDialog == null )
    //                    chanceDialog = new HitChanceDialog();

    //                chanceDialog.windowRect = windowLoc;
    //                Find.WindowStack.Add( chanceDialog );
    //                //  FloatMenuOption floatMenu = new FloatMenuOption( "Chance to hit:" );
    //            }
    //        }
    //        else if ( chanceDialog != null )
    //        {
    //            chanceDialog.Close();
    //            chanceDialog = null;
    //        }
    //    }
    //}

    //public class HitChanceDialog : Window
    //{
    //    public override Vector2 InitialSize
    //    {
    //        get
    //        {
    //            return new Vector2( 50f, 100f );
    //        }
    //    }

    //    public override void DoWindowContents( Rect inRect )
    //    {
    //        List<Pawn> m_pawns = new List<Pawn>();
    //        m_pawns.AddRange( Find.VisibleMap.mapPawns.FreeColonists );
    //        Listing_Standard listing_Standard = new Listing_Standard( inRect );

    //        foreach( var p in m_pawns )
    //        {
    //            Log.Message( "shot:" + Verse.TooltipUtility.ShotCalculationTipString( p ) );
    //            listing_Standard.Label( Verse.TooltipUtility.ShotCalculationTipString( p ));
    //            break;
    //        }
    //        listing_Standard.End();
    //    }
    //}
}

//// Verse.ShotReport
//public static ShotReport HitReportFor( Thing caster, Verb verb, LocalTargetInfo target )
//{
//    Pawn pawn = caster as Pawn;
//    IntVec3 cell = target.Cell;
//    ShotReport result;
//    result.distance = ( cell - caster.Position ).LengthHorizontal;
//    result.target = target.ToTargetInfo( caster.Map );
//    float f;
//    if( pawn != null )
//    {
//        f = pawn.GetStatValue( StatDefOf.ShootingAccuracy, true );
//    }
//    else
//    {
//        f = 0.96f;
//    }
//    result.factorFromShooterAndDist = Mathf.Pow( f, result.distance );
//    if( result.factorFromShooterAndDist < 0.0201f )
//    {
//        result.factorFromShooterAndDist = 0.0201f;
//    }
//    result.factorFromEquipment = verb.verbProps.GetHitChanceFactor( verb.ownerEquipment, result.distance );
//    result.covers = CoverUtility.CalculateCoverGiverSet( cell, caster.Position, caster.Map );
//    result.coversOverallBlockChance = CoverUtility.CalculateOverallBlockChance( cell, caster.Position, caster.Map );
//    if( !caster.Position.Roofed( caster.Map ) && !target.Cell.Roofed( caster.Map ) )
//    {
//        result.factorFromWeather = caster.Map.weatherManager.CurWeatherAccuracyMultiplier;
//    }
//    else
//    {
//        result.factorFromWeather = 1f;
//    }
//    result.factorFromTargetSize = 1f;
//    if( target.HasThing )
//    {
//        Pawn pawn2 = target.Thing as Pawn;
//        if( pawn2 != null )
//        {
//            result.factorFromTargetSize = pawn2.BodySize;
//        }
//        else
//        {
//            result.factorFromTargetSize = target.Thing.def.fillPercent * 1.7f;
//        }
//        result.factorFromTargetSize = Mathf.Clamp( result.factorFromTargetSize, 0.5f, 2f );
//    }
//    result.forcedMissRadius = verb.verbProps.forcedMissRadius;
//    return result;
//}
