using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HugsLib;
using System;

namespace WRS
{

    //public class Weapons_GUI : ModBase
    //{
    //    private static List<IntVec3> m_ringCellsShort = new List<IntVec3>();
    //    private static List<IntVec3> m_ringCellsMedium = new List<IntVec3>();
    //    private static List<IntVec3> m_ringCellsLong = new List<IntVec3>();
        
    //    public enum PawnType
    //    {
    //        Colonists,
    //        Raiders,
    //        None
    //    };

    //    public static bool PawnsDirty = true;
    //    public static PawnType TypeOfPawn = PawnType.None;

    //    protected List<Pawn> m_pawns = new List<Pawn>();


    //    // private static List<IntVec3> pawnWeaponRings = new IntVec3( 0, 0, 0 );

    //    private static Dictionary<Name, Pair<IntVec3, List<IntVec3>>> m_pawnWeaponRings = new Dictionary<Name, Pair<IntVec3, List<IntVec3>>>();

    //    private static void UpdatePawnWeaponRing( Pawn pawn, Pair<IntVec3, List<IntVec3>> loc_rad)
    //    {
    //        //int num = 0;

    //        foreach( var v in pawn.equipment.Primary.def.Verbs )
    //        {
    //            int num = GenRadial.NumCellsInRadius( v.range );
    //            for( int i = 0,
    //                numShort = GenRadial.NumCellsInRadius( 5 ),
    //                numMedium = GenRadial.NumCellsInRadius( 20 ); i < num; i++ )
    //            {
    //                if( i < numShort )
    //                {
    //                    loc_rad.Second.Add( pawn.Position + GenRadial.RadialPattern[ i ] );
    //                }
    //                else if( i < numMedium )
    //                {
    //                    loc_rad.Second.Add( pawn.Position + GenRadial.RadialPattern[ i ] );
    //                }
    //                else
    //                {
    //                    loc_rad.Second.Add( pawn.Position + GenRadial.RadialPattern[ i ] );
    //                }
    //            }

    //        }

    //        m_pawnWeaponRings.Add( pawn.Name, loc_rad );
    //    }

    //    public override string ModIdentifier
    //    {
    //        get
    //        {
    //            return "Weapons_GUI";
    //        }
    //    }

    //    public override void Update()
    //    {
    //        if( Current.Game == null )
    //            return;

    //        if( PawnsDirty )
    //        {
    //            Log.Message( " Pawns Dirty" );
    //            m_pawnWeaponRings.Clear();
    //            m_pawns.Clear();
    //            PawnsDirty = false;

    //            Log.Message( " Pawns Cleared" );
    //            if( TypeOfPawn == Weapons_GUI.PawnType.Colonists )
    //            {
    //                m_pawns.AddRange( Find.VisibleMap.mapPawns.FreeColonists );
    //            }
    //            else if ( TypeOfPawn == Weapons_GUI.PawnType.Raiders )
    //            {
    //                //m_pawns.AddRange( Find.VisibleMap.mapPawns.AllPawns );
    //            }
    //            else if( TypeOfPawn == Weapons_GUI.PawnType.None )
    //            {
    //                return;
    //            }
    //        }

    //        Pair<IntVec3, List<IntVec3>> loc_rad_pair;
    //        foreach( var tp in this.m_pawns )
    //        {
    //            var item = tp.equipment.Primary;
    //            if( item != null && item.def.IsRangedWeapon )
    //            {
    //                if( m_pawnWeaponRings.TryGetValue( tp.Name, out loc_rad_pair ) )
    //                {
    //                    if( tp.Position != loc_rad_pair.First )
    //                    {
    //                        Log.Message( tp.Name + " Update attempt " );
    //                        m_pawnWeaponRings.Remove( tp.Name );
    //                        loc_rad_pair = new Pair<IntVec3, List<IntVec3>>( tp.Position, new List<IntVec3>() );
    //                        UpdatePawnWeaponRing( tp, loc_rad_pair );
    //                        Log.Message( tp.Name + " Updated " );
    //                    }
    //                }
    //                else
    //                {
    //                    loc_rad_pair = new Pair<IntVec3, List<IntVec3>>( tp.Position, new List<IntVec3>() );
    //                    UpdatePawnWeaponRing( tp, loc_rad_pair );
    //                    Log.Message( tp.Name + " Added" );
    //                }
    //            }
    //            else
    //            {
    //                //Log.Message( tp.Name + " has no range weapon " );
    //                // No ranged weapon, no circle
    //                m_pawnWeaponRings.Remove( tp.Name );
    //            }
    //        }



    //        foreach( var cells in m_pawnWeaponRings )
    //        {
    //            Log.Message( " Draw Cells" );
    //            GenDraw.DrawFieldEdges( cells.Value.Second, Color.white );
    //        }
    //     ///   GenDraw.DrawFieldEdges( m_ringCellsLong, Color.green );
    //     //   GenDraw.DrawFieldEdges( m_ringCellsMedium, Color.white );
    //     //   GenDraw.DrawFieldEdges( m_ringCellsShort, Color.red );

    //    }

    //}







    //    foreach ( var v in tp.equipment.Primary.def.Verbs )
    //    {
    //        Log.Message( tp.Name + " with " + tp.verbTracker.PrimaryVerb.ToString() + " r:" + v.range );
    //        //    foreach( var vv in tp.verbTracker.AllVerbs )
    //        {
    //            //        Log.Message( tp.Name + " verb " + vv.ToString() );

    //        }
    //        //Find.Targeter.BeginTargeting( tp.verbTracker );
    //        //  tp.verbTracker.PrimaryVerb
    //        DrawWeaponRing( tp.Position, v.range );
    //        //     DrawTargetingHighlight_Cell( tp.Position );
    //    }
    //public class Weapons_Tab : MainTabWindow_PawnList
    //{

    //    public override Vector2 RequestedTabSize => new Vector2( 200f, 200f );


    //    // Use default for now.
    //    protected override void BuildPawnList()
    //    {
    //    }

    //    private static void DrawTargetingHighlight_Cell( IntVec3 c )
    //    {
    //        Vector3 position = c.ToVector3ShiftedWithAltitude( AltitudeLayer.Building );
    //        Graphics.DrawMesh( MeshPool.plane10, position, Quaternion.identity, GenDraw.CurTargetingMat, 0 );
    //    }

    //    public override void DoWindowContents( Rect rect )
    //    {
    //        base.DoWindowContents( rect );

    //        var position = new Rect( 0f, 0f, rect.width, 80f );
    //        GUI.BeginGroup( position );

    //        // prisoner / colonist / Animal toggle
    //        var sourceButton = new Rect( 0f, 0f, 200f, 35f );
    //        if( Widgets.ButtonText( sourceButton, "Weapon Range Circles" ) )
    //        {
    //            List<FloatMenuOption> options = new List<FloatMenuOption>();
    //            options.Add( new FloatMenuOption( "Colonists"/*.Translate()*/, delegate
    //            {
    //                Weapons_GUI.PawnsDirty = true;
    //                Weapons_GUI.TypeOfPawn = Weapons_GUI.PawnType.Colonists;
    //            } ) );
    //            options.Add( new FloatMenuOption( "Raiders"/*.Translate()*/, delegate
    //            {
    //                Weapons_GUI.PawnsDirty = true;
    //                Weapons_GUI.TypeOfPawn = Weapons_GUI.PawnType.Raiders;
    //            } ) );
    //            options.Add( new FloatMenuOption( "None"/*.Translate()*/, delegate
    //            {
    //                Weapons_GUI.PawnsDirty = true;
    //                Weapons_GUI.TypeOfPawn = Weapons_GUI.PawnType.None;
    //            } ) );
    //            Find.WindowStack.Add( new FloatMenu( options ) );
    //        }
    //        GUI.EndGroup();

    //    }

//        protected override void DrawPawnRow( Rect rect, Pawn p )
  //      {
            //foreach( var tp in pawns )
            //{
            //    GenDraw.DrawRadiusRing( tp.Position, 200 );
            //}

            //var bloodRect = new Rect( 5, 5, 100f, 100f );
            //Widgets.DrawHighlightIfMouseover( bloodRect );
            //GUI.DrawTexture( bloodRect, Widgets.CheckboxOnTex );

            //// name is handled in PreDrawRow, start at 175
            //var x = 175f;
            //var y = rect.yMin;

            //// care
            //var careRect = new Rect(x, y, 100f, 30f);
            //Utility_Medical.MedicalCareSetter(careRect, ref p.playerSettings.medCare);
            //x += 100f;

            //// blood
            //var bloodRect = new Rect(x, y, 50f, 30f);
            //var bleedRate = p.health.hediffSet.BleedingRate; // float in range 0 - 1
            //float iconSize;
            //if (bleedRate < 0.01f)
            //{
            //    iconSize = 0f;
            //}
            //else if (bleedRate < .1f)
            //{
            //    iconSize = 8f;
            //}
            //else if (bleedRate < .3f)
            //{
            //    iconSize = 16f;
            //}
            //else
            //{
            //    iconSize = 24f;
            //}
            //var iconRect = Inner(bloodRect, iconSize);
            //GUI.DrawTexture(iconRect, Utility_Medical.BloodTexture);
            //TooltipHandler.TipRegion(bloodRect,
            //                          "BleedingRate".Translate() + ": " + bleedRate.ToStringPercent() + "/" +
            //                          "LetterDay".Translate());
            //Widgets.DrawHighlightIfMouseover(bloodRect);
            //x += 50f;

            //// Operations
            //var opLabel = new Rect(x, y, 50f, 30f);
            //if (Widgets.ButtonInvisible(opLabel))
            //{
            //    if (Event.current.button == 0)
            //    {
            //        Utility_Medical.RecipeOptionsMaker(p);
            //    }
            //    else if (Event.current.button == 1)
            //    {
            //        p.BillStack.Clear();
            //    }
            //}
            //var opLabelString = new StringBuilder();
            //opLabelString.AppendLine("FluffyMedical.ClickTo".Translate("FluffyMedical.ScheduleOperation".Translate()));
            //opLabelString.AppendLine(
            //    "FluffyMedical.RightClickTo".Translate("FluffyMedical.UnScheduleOperations".Translate()));
            //opLabelString.AppendLine();
            //opLabelString.AppendLine("FluffyMedical.ScheduledOperations".Translate());

            //var opScheduled = false;
            //foreach (var op in p.BillStack)
            //{
            //    opLabelString.AppendLine(op.LabelCap);
            //    opScheduled = true;
            //}

            //if (opScheduled)
            //{
            //    GUI.DrawTexture(Inner(opLabel, 16f), Widgets.CheckboxOnTex);
            //}
            //else
            //{
            //    opLabelString.AppendLine("FluffyMedical.NumCurrentOperations".Translate("No"));
            //}

            //TooltipHandler.TipRegion(opLabel, opLabelString.ToString());
            //Widgets.DrawHighlightIfMouseover(opLabel);
            //x += 50f;

            //// main window
            //Text.Anchor = TextAnchor.MiddleCenter;
            //var colWidth = (rect.width - x) / CapDefs.Count;
            //foreach (PawnCapacityDef t in CapDefs)
            //{
            //    var capDefCell = new Rect(x, y, colWidth, 30f);
            //    var colorPair = HealthCardUtility.GetEfficiencyLabel(p, t);
            //    var label = (p.health.capacities.GetEfficiency(t) * 100f).ToString("F0") + "%";
            //    GUI.color = colorPair.Second;
            //    Widgets.Label(capDefCell, label);
            //    if (Mouse.IsOver(capDefCell))
            //    {
            //        GUI.DrawTexture(capDefCell, TexUI.HighlightTex);
            //    }
            //    Utility_Medical.DoHediffTooltip(capDefCell, p, t);
            //    x += colWidth;
            //}
 //       }
 //   }
}
