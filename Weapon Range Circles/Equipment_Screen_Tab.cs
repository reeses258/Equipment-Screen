using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Linq;


namespace WRS
{
    public struct ArmorValues
    {
        public ArmorValues( Thing t )
        {
            Armor = new List<Thing>();
            Armor.Add( t );
            Blunt = 0;
            Sharp = 0;
            Heat = 0;
            Electric = 0;
        }
        public float Blunt;
        public float Sharp;
        public float Heat;
        public float Electric;
        public List< Thing > Armor;
    }
    public class EquipmentTab : MainTabWindow_PawnList
    {
        public override Vector2 RequestedTabSize => new Vector2( 1200f, 800f );

        private static Vector2 PawnTextureRatio = new Vector2( 46f, 72f );

        private static Pawn ActivePawn = null;
        private static List<ThingWithComps> ActiveWeapon = new List<ThingWithComps>();
        private static List<Thing> ActiveHelmet = new List<Thing>();
        private static List<Thing> ActiveSkinApparel = new List<Thing>();
        private static List<Thing> ActiveShellApparel = new List<Thing>();
        private static List<Thing> ActiveMiddleApparel = new List<Thing>();
        private static List<Thing> ActiveAcessory = new List<Thing>();
        private static Thing ProjectedItem;

        private static float LabelSpacing = 25f;

        private Rect ColonistSelection = new Rect();
        private Rect ColonistRect = new Rect();
        private Rect OffensiveStatsRect = new Rect();
        private Rect AvailableEquipmentRect = new Rect();
        private Rect DefensiveStatsRect = new Rect();

        private float x = 0f;
        private float y = 0f;
        private bool isRanged = false;
        private bool isRangeMeleeMix = false;
        private bool isProjected = false;
        private Vector2 scollPosition = Vector2.zero;

        // Use default for now.
        protected override void BuildPawnList()
        {
            pawns.Clear();
            pawns.AddRange( Find.VisibleMap.mapPawns.FreeColonists );
        }

        public void SetInitialWindowSettings()
        {
            GUI.contentColor = Color.white;
            Text.Font = GameFont.Small;
            GUI.skin.box.wordWrap = true;
        }

        public override void DoWindowContents( Rect rect )
        {
            base.DoWindowContents( rect );

            SetInitialWindowSettings();

            // Initialize View Rects
            ColonistSelection = new Rect( 0f, 0f, rect.width, 35f );

            ColonistRect = new Rect( 0f, ColonistSelection.height, rect.width / 3, rect.height / 1.5f );
            ColonistRect.width = ColonistRect.height * PawnTextureRatio[ 0 ] / PawnTextureRatio[ 1 ];

            OffensiveStatsRect = new Rect( ColonistRect.x + ColonistRect.width + 10, ColonistRect.y,
                ( rect.width - ( ColonistRect.x + ColonistRect.width + 10 ) ) / 2, ColonistRect.height );
            DefensiveStatsRect = new Rect( OffensiveStatsRect.x + OffensiveStatsRect.width + 10, ColonistRect.y,
                OffensiveStatsRect.width, ColonistRect.height );
            AvailableEquipmentRect = new Rect( ColonistRect.x, ColonistRect.y + ColonistRect.height + 10,
                rect.width - ( ColonistRect.x * 2 ), rect.height - ColonistRect.y + ColonistRect.height + 10 );

            SetActiveEquipment();
            GenerateColonistSelectionBar();
            GenerateColonistImage();
            GenerateOffensiveStats();
            GenerateDefensiveStats();
            GenerateAvailableEquipement();
        }

        public void SetActiveEquipment()
        {
            if( ActivePawn == null )
                return;

            ActiveWeapon = ActivePawn.equipment.AllEquipment;

            ActiveHelmet.Clear();
            ActiveSkinApparel.Clear();
            ActiveMiddleApparel.Clear();
            ActiveShellApparel.Clear();
            ActiveAcessory.Clear();

            // Clothing
            foreach( var v in ActivePawn.apparel.WornApparel )
            {
                foreach( var l in v.def.apparel.layers )
                {
                    if( l == ApparelLayer.Overhead )
                        ActiveHelmet.Add( v );
                    else if( l == ApparelLayer.OnSkin )
                        ActiveSkinApparel.Add( v );
                    else if( l == ApparelLayer.Middle )
                        ActiveMiddleApparel.Add( v );
                    else if( l == ApparelLayer.Shell )
                        ActiveShellApparel.Add( v );
                    else if( l == ApparelLayer.Accessory )
                        ActiveAcessory.Add( v );
                }
            }
        }

        public void GenerateColonistSelectionBar()
        {
            GUI.BeginGroup( ColonistSelection );

            if( Widgets.ButtonImage( new Rect( ColonistSelection.width / 2f - 100f - 40f, 0f, 35f, 35f ), TexUI.ArrowTexLeft ) )
            {
                Pawn oldP = pawns[ 0 ];
                bool found = false;

                if( ( ActivePawn == null || ActivePawn == oldP ) && pawns.Count > 0 )
                {
                    ActivePawn = pawns[ pawns.Count - 1 ];
                    found = true;
                }


                foreach( var p in pawns )
                {
                    if( found )
                        break;

                    if( p == ActivePawn )
                    {
                        ActivePawn = oldP;
                        found = true;
                        break;
                    }
                    oldP = p;
                }
            }

            var colonistSelector = new Rect( ColonistSelection.width / 2f - 100f, 0f, 200f, 35f );
            if( ActivePawn == null )
            {
                if( Widgets.ButtonText( colonistSelector, "Colonists" ) )
                {
                    BuildColonistButton();
                }
            }
            else
            {
                if( Widgets.ButtonText( colonistSelector, ActivePawn.NameStringShort ) )
                {
                    BuildColonistButton();
                }
            }

            if( Widgets.ButtonImage( new Rect( ColonistSelection.width / 2f + 100f + 5f, 0f, 35f, 35f ), TexUI.ArrowTexRight ) )
            {
                bool next = false;
                bool found = false;

                foreach( var p in pawns )
                {
                    if( next )
                    {
                        ActivePawn = p;
                        found = true;
                        break;
                    }
                    if( p == ActivePawn )
                        next = true;
                }

                if( !found )
                {
                    if( !next )
                        ActivePawn = pawns[ 0 ];
                    else
                        ActivePawn = pawns[ pawns.Count - 1 ];
                }

            }

            GUI.EndGroup();
        }

        public void GenerateColonistImage()
        {
            if( ActivePawn == null )
                return;

            this.DrawColonist( ColonistRect, ActivePawn, Find.VisibleMap );

            GUI.BeginGroup( ColonistRect );

            float boxSize = 80f;

            Rect WeaponsRect = new Rect( 5f, 5f, boxSize, boxSize * ActiveWeapon.Count );
            Rect HelmetsRect = new Rect( ColonistRect.width / 2 - ( boxSize / 2 ), 5f, boxSize * ActiveHelmet.Count, boxSize );
            Rect AccessoryApparelRect = new Rect( ColonistRect.width - boxSize - 5f, 5f, boxSize * ActiveAcessory.Count, boxSize );
            Rect SkinApparelRect = new Rect( 5f, ColonistRect.height / 3, boxSize, boxSize * ActiveSkinApparel.Count );
            Rect MiddleApparelRect = new Rect( ( ColonistRect.width / 2 ) - ( boxSize / 2 ), ColonistRect.height / 2 - ( boxSize / 2 ), boxSize, boxSize * ActiveMiddleApparel.Count );
            Rect ShellApparelRect = new Rect( ColonistRect.width - boxSize - 5f, ColonistRect.height / 2 - ( boxSize / 2 ), boxSize, boxSize * ActiveMiddleApparel.Count );

            // Weapon Box
            y = WeaponsRect.y;
            x = WeaponsRect.x;
            foreach( var w in ActiveWeapon )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            // Helmet Box
            y = HelmetsRect.y;
            x = HelmetsRect.x;
            foreach( var w in ActiveHelmet )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            // Skin box
            y = SkinApparelRect.y;
            x = SkinApparelRect.x;
            foreach( var w in ActiveSkinApparel )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            // Middle Box
            y = MiddleApparelRect.y;
            x = MiddleApparelRect.x;
            foreach( var w in ActiveMiddleApparel )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            // Shell Box
            y = ShellApparelRect.y;
            x = ShellApparelRect.x;
            foreach( var w in ActiveShellApparel )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            // Accessory Box
            y = AccessoryApparelRect.y;
            x = AccessoryApparelRect.x;
            foreach( var w in ActiveAcessory )
            {
                CreateStorageBox( new Rect( x, y, boxSize, boxSize ), w );
                y += boxSize;
            }

            GUI.EndGroup();
        }

        public void GenerateOffensiveStats()
        {
            if( ActivePawn == null )
                return;

            GUI.BeginGroup( OffensiveStatsRect );
            x = 0f;
            y = 0f;

            Widgets.ListSeparator( ref y, OffensiveStatsRect.width, "Offense"/*.Translate()*/ );

            // Weapons & Attack details
            GenerateAttackDetails();

            GUI.EndGroup();
        }

        public Rect PrintRectActiveDefensive()
        {
            return new Rect( x /*+ DefensiveStatsRect.width / 2*/, y, DefensiveStatsRect.width / 3 - 60f, LabelSpacing );
        }

        public Rect PrintRectProjDefensive()
        {
            return new Rect( x + ( DefensiveStatsRect.width / 3 ) - 60f, y, 60f, LabelSpacing );
        }

        public void PrintThingIcons( Rect rect, List< Thing > thingList )
        {
            float x = rect.x;
            float size = 20f;

            foreach( var item in thingList )
            {
                Widgets.ThingIcon( new Rect( x, rect.y, size, size ), item, 1f );
                x += size + 1;
            }
        }


        public void GenerateDefensiveStats()
        {

            if( ActivePawn == null )
                return;

            GUI.BeginGroup( DefensiveStatsRect );
            x = 0f;
            y = 0f;

            //foreach( BodyPartRecord bPR in BodyDefOf.Human.AllParts )
            //{
            //    if( x > 0f )
            //        Log.Message( " indented" );
            //    Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 2, LabelSpacing ), bPR.ToString() );  
            //    if( y + LabelSpacing >= DefensiveStatsRect.height && x == 0 )
            //    {
            //        x = DefensiveStatsRect.width / 2;
            //        Log.Message( "Cut n Half" );
            //    }
            //    y += LabelSpacing;
            //}

            Widgets.ListSeparator( ref y, DefensiveStatsRect.width, "Defense"/*.Translate()*/ );

            List<Pair<BodyPartGroupDef, ArmorValues>> individualParts = new List<Pair<BodyPartGroupDef, ArmorValues>>();
            List<Pair<BodyPartGroupDef, ArmorValues>> individualPartsProjected = new List<Pair<BodyPartGroupDef, ArmorValues>>();

            // Clothing
            bool found = false;
            isProjected = ProjectedItem != null && ProjectedItem.def.IsApparel;
            isRangeMeleeMix = false;
            ArmorValues tmp = new ArmorValues();

            foreach( var curApparel in ActivePawn.apparel.WornApparel )
            {
                foreach( var part in curApparel.def.apparel.bodyPartGroups )
                {
                    found = false;
                    for( var i = 0; !found && i < individualParts.Count(); i++ )
                    {
                        if( individualParts[ i ].First.defName == part.defName )
                        {
                            tmp = individualParts[ i ].Second;
                            tmp.Blunt += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Blunt, true ) );
                            tmp.Sharp += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Sharp, true ) );
                            tmp.Heat += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Heat, true ) );
                            tmp.Electric += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Electric, true ) );
                            tmp.Armor.Add( curApparel );

                            individualParts[ i ] = new Pair<BodyPartGroupDef, ArmorValues>( individualParts[ i ].First, tmp );
                            found = true;
                            if( isProjected && ApparelUtility.CanWearTogether( curApparel.def, ProjectedItem.def ) )
                            {
                                tmp = individualPartsProjected[ i ].Second;
                                tmp.Blunt += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Blunt, true ) );
                                tmp.Sharp += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Sharp, true ) );
                                tmp.Heat += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Heat, true ) );
                                tmp.Electric += Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Electric, true ) );
                                tmp.Armor.Add( curApparel );

                                individualPartsProjected[ i ] = new Pair<BodyPartGroupDef, ArmorValues>( individualPartsProjected[ i ].First, tmp );
                                found = true;
                            }
                        }
                    }
                    if( !found )
                    {
                        tmp = new ArmorValues();
                        tmp.Blunt = Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Blunt, true ) );
                        tmp.Sharp = Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Sharp, true ) );
                        tmp.Heat = Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Heat, true ) );
                        tmp.Electric = Mathf.Clamp01( curApparel.GetStatValue( StatDefOf.ArmorRating_Electric, true ) );
                        tmp.Armor.Add( curApparel );

                        individualParts.Add(new Pair<BodyPartGroupDef, ArmorValues>(part, tmp));
                        if ( isProjected && ApparelUtility.CanWearTogether( curApparel.def, ProjectedItem.def ) )
                            individualPartsProjected.Add( new Pair<BodyPartGroupDef, ArmorValues>( part, tmp ) );
                        else
                            individualPartsProjected.Add( new Pair<BodyPartGroupDef, ArmorValues>( part, new ArmorValues( curApparel ) ) );
                    }
                }

            }

            if( isProjected )
            {
                for( int i = 0; i < ProjectedItem.def.apparel.bodyPartGroups.Count; i++ )
                {
                    found = false;
                    for( int k = 0; !found && k < individualPartsProjected.Count(); k++ )
                    {
                        if( ProjectedItem.def.apparel.bodyPartGroups[ i ].defName == individualPartsProjected[ k ].First.defName )
                        {
                            tmp = individualPartsProjected[ k ].Second;
                            tmp.Blunt += Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Blunt, true ) );
                            tmp.Sharp += Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Sharp, true ) );
                            tmp.Heat += Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Heat, true ) );
                            tmp.Electric += Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Electric, true ) );
                            individualPartsProjected[ k ] = new Pair<BodyPartGroupDef, ArmorValues>( individualPartsProjected[ k ].First, tmp );
                            found = true;
                        }
                    }
                    if( !found )
                    {
                        tmp = new ArmorValues();
                        tmp.Blunt = Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Blunt, true ) );
                        tmp.Sharp = Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Sharp, true ) );
                        tmp.Heat = Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Heat, true ) );
                        tmp.Electric = Mathf.Clamp01( ProjectedItem.GetStatValue( StatDefOf.ArmorRating_Electric, true ) );

                        individualPartsProjected.Add( new Pair<BodyPartGroupDef, ArmorValues>( ProjectedItem.def.apparel.bodyPartGroups[ i ], tmp ) );
                        individualParts.Add( new Pair<BodyPartGroupDef, ArmorValues>( ProjectedItem.def.apparel.bodyPartGroups[ i ], new ArmorValues()) );
                    }

                }
            }

            float yTmp = y;
            for( int i = 0; i < individualParts.Count; i++ )
            {
                GUI.contentColor = new Color( 0.8f, 0.8f, 0.8f, 1f );
                Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 3, LabelSpacing ), ""/*.Translate()*/ + individualParts[ i ].First.LabelCap );
                y += LabelSpacing;
                GUI.contentColor = Color.white;

                if( individualParts[ i ].Second.Blunt > .0005 || individualPartsProjected[ i ].Second.Blunt > .0005 )
                {
                    Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 3, LabelSpacing ), "Blunt" );
                    PrintActiveStat( PrintRectActiveDefensive(), individualParts[ i ].Second.Blunt, true );
                    PrintThingIcons( PrintRectActiveDefensive(), individualPartsProjected[ i ].Second.Armor );
                    PrintProjectedStat( PrintRectProjDefensive(), individualParts[ i ].Second.Blunt, individualPartsProjected[ i ].Second.Blunt, false, true );
                    y += LabelSpacing;
                }

                if( y >= DefensiveStatsRect.height )
                {
                    y = yTmp;
                    x += DefensiveStatsRect.width / 3 + 10;
                }

                if( individualParts[ i ].Second.Sharp > .0005 || individualPartsProjected[ i ].Second.Sharp > .0005 )
                {
                    Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 3, LabelSpacing ), "Sharp" );
                    PrintActiveStat( PrintRectActiveDefensive(), individualParts[ i ].Second.Sharp, true );
                    PrintThingIcons( PrintRectActiveDefensive(), individualPartsProjected[ i ].Second.Armor );
                    PrintProjectedStat( PrintRectProjDefensive(), individualParts[ i ].Second.Sharp, individualPartsProjected[ i ].Second.Sharp, false, true );
                    y += LabelSpacing;
                }

                if( y >= DefensiveStatsRect.height )
                {
                    y = yTmp;
                    x += DefensiveStatsRect.width / 3 + 10;
                }

                if( individualParts[ i ].Second.Heat > .0005 || individualPartsProjected[ i ].Second.Heat > .0005 )
                {
                    Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 3, LabelSpacing ), "Heat" );
                    PrintActiveStat( PrintRectActiveDefensive(), individualParts[ i ].Second.Heat, true );
                    PrintThingIcons( PrintRectActiveDefensive(), individualPartsProjected[ i ].Second.Armor );
                    PrintProjectedStat( PrintRectProjDefensive(), individualParts[ i ].Second.Heat, individualPartsProjected[ i ].Second.Heat, false, true );
                    y += LabelSpacing;
                }

                if( y >= DefensiveStatsRect.height )
                {
                    y = yTmp;
                    x += DefensiveStatsRect.width / 3 + 10;
                }

                if( individualParts[ i ].Second.Electric> .0005 || individualPartsProjected[ i ].Second.Electric > .0005 )
                {
                    Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 3, LabelSpacing ), "Electric" );
                    PrintActiveStat( PrintRectActiveDefensive(), individualParts[ i ].Second.Electric, true );
                    PrintThingIcons( PrintRectActiveDefensive(), individualPartsProjected[ i ].Second.Armor );
                    PrintProjectedStat( PrintRectProjDefensive(), individualParts[ i ].Second.Electric, individualPartsProjected[ i ].Second.Electric, false, true );
                    y += LabelSpacing;
                }
                
                if( y >= DefensiveStatsRect.height )
                {
                    y = yTmp;
                    x += DefensiveStatsRect.width / 3 + 10;
                }

            }



            //   Log.Message( v.LabelCap + " armor " + rtn + " coverage " + v.def.apparel.HumanBodyCoverage + " group " + v.def.apparel.bodyPartGroups.First().ToString() );
            //Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 2, LabelSpacing ), v.LabelCap + num +" Blunt armor" );
            //Widgets.Label( new Rect( x, y, DefensiveStatsRect.width / 2, LabelSpacing ), " Blunt Armor " + num );

            GUI.EndGroup();
        }

        public void GenerateAvailableEquipement()
        {
            if( ActivePawn == null )
                return;

            List<Thing> availableEquipment = new List<Thing>();
            availableEquipment.AddRange( ActivePawn.Map.listerThings.ThingsInGroup( ThingRequestGroup.Weapon ) );
            availableEquipment.AddRange( ActivePawn.Map.listerThings.ThingsInGroup( ThingRequestGroup.Apparel ) );

            GUI.BeginGroup( AvailableEquipmentRect );
            x = 0f;
            y = 0f;

            CreateStorageBoxList( new Rect( 0, 0, AvailableEquipmentRect.width, AvailableEquipmentRect.height ), availableEquipment );
            //foreach( var l in list )
            //{
            //    y += LabelSpacing; ;
            //    Widgets.Label( new Rect( x, y, 100f, LabelSpacing ), l.ToString() );
            //}
            GUI.EndGroup();
        }

        public void CreateStorageBoxList( Rect storage, List<Thing> storageList )
        {
            float height = 80f;
            float width = 80f;
            int columns = Mathf.FloorToInt( storage.width / width );
            int rows = Mathf.CeilToInt( storageList.Count() / ( columns != 0 ? columns : 1 ) );

            Rect currentBox = new Rect();
            Rect scrollingRect = new Rect( 5, 5, storage.width - 21f, ( height * rows < storage.height ? storage.height + 50f : height * rows ) );

            Log.Message( " rec " + scrollingRect.height + " rows: " + rows );
            Widgets.BeginScrollView( storage, ref scrollPosition, scrollingRect );

            x = scrollingRect.x;
            foreach( var s in storageList )
            {
                currentBox = new Rect( x, y, height, width );
                CreateStorageBox( currentBox, s, true );
                x += width;
                if( x + width >= scrollingRect.width )
                {
                    x = scrollingRect.x;
                    y += height;
                    if( y >= scrollingRect.height )
                    {
                        // Out of room
                        break;
                    }
                }
            }
            Widgets.EndScrollView();
        }

        public void CreateStorageBox( Rect storageBox, Thing item, bool activateMouse = false )
        {
            //Widgets.InfoCardButton( storageBox.width, 0f, item );

            if( item != null )
            {
                Widgets.ThingIcon( storageBox, item, 1f );
                GUI.Box( storageBox, item.LabelCap );
            }
            else
            {
                GUI.Box( storageBox, "" );
            }

            if( activateMouse && Mouse.IsOver( storageBox ) )
            {
                ProjectedItem = item;
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                if( Input.GetMouseButton( 0 ) || Input.GetMouseButton( 1 ) )
                {
                    options.Add( new FloatMenuOption( "Equip", delegate
                   {
                       Verse.AI.Job job = new Verse.AI.Job( JobDefOf.Wear, item );

                       if( item.def.IsApparel )
                           job = new Verse.AI.Job( JobDefOf.Wear, item );
                       else
                           job = new Verse.AI.Job( JobDefOf.Equip, item );

                       ActivePawn.jobs.StartJob( job );
                   } ) );
                    Find.WindowStack.Add( new FloatMenu( options ) );
                }
            }
            //CreateCompareItemLabelToolTip( item );
            //TooltipHandler.TipRegion( storageBox, "DropThing".Translate() );
        }

        public bool SetGuiColorForDetails( float active, float project )
        {
            if( active > project )
                GUI.contentColor = Color.red;
            else if( active < project )
                GUI.contentColor = Color.green;
            else
                return false;
            return true;
        }

        private void PrintStatName( string name )
        {
            Widgets.Label( new Rect( x, y, OffensiveStatsRect.width / 2, LabelSpacing ), name );
        }

        private void PrintActiveStat( Rect activeRect, float activeValue, bool percent = false )
        {
            if( !isRangeMeleeMix )
            {
                GUI.skin.label.alignment = TextAnchor.UpperRight;
                if( percent )
                    GUI.Label( activeRect, "" + activeValue.ToStringPercent() );
                else
                    GUI.Label( activeRect, "" + activeValue.ToStringDecimalIfSmall() );

                GUI.skin.label.alignment = TextAnchor.UpperLeft;
            }
        }

        private void PrintProjectedStat( Rect printRect, float activeValue, float projectedValue, bool flipped = false, bool percent = false )
        {
            if( isProjected )
            {
                if( flipped )
                {
                    if( !SetGuiColorForDetails( projectedValue, activeValue ) )
                        return;
                }
                else
                {
                    if( !SetGuiColorForDetails( activeValue, projectedValue ) )
                        return;
                }

                if( percent )
                    Widgets.Label( printRect, " => " + projectedValue.ToStringPercent() );
                else
                    Widgets.Label( printRect, " => " + projectedValue.ToStringDecimalIfSmall() );

                GUI.contentColor = Color.white;
            }
        }

        public Rect PrintRectActiveOffensive()
        {
            return new Rect( x + OffensiveStatsRect.width / 2, y, OffensiveStatsRect.width / 2 / 2, LabelSpacing );
        }

        public Rect PrintRectProjOffensive()
        {
            return new Rect( x + OffensiveStatsRect.width / 2 + OffensiveStatsRect.width / 2 / 2, y, OffensiveStatsRect.width / 2 / 2, LabelSpacing );
        }

        public void GenerateAttackDetails()
        {
            if( ActiveWeapon.Count == 0 || ActivePawn == null )
                return;

            float activeValue = 0f;
            float projValue = 0f;

            isProjected = ProjectedItem != null && ProjectedItem.def.IsWeapon;
            if( isProjected )
            {
                isRangeMeleeMix = ActiveWeapon[ 0 ].def.IsRangedWeapon != ProjectedItem.def.IsRangedWeapon;
                isRanged = ProjectedItem.def.IsRangedWeapon;
            }
            else
            {
                isRangeMeleeMix = false;
                isRanged = ActiveWeapon[ 0 ].def.IsRangedWeapon;
            }

            Widgets.Label( new Rect( x, y, OffensiveStatsRect.width, LabelSpacing ), "" + ActiveWeapon[ 0 ].LabelCap );
            y += LabelSpacing;

            if(  isRanged  )
            {
                PrintStatName( "Damage" );
                activeValue = Equipment_Ranged_Calculators.Damage( ActiveWeapon[ 0 ].def );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.Damage( ProjectedItem.def );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Warmup" );
                activeValue = Equipment_Ranged_Calculators.Warmup( ActiveWeapon[ 0 ].def );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.Warmup( ProjectedItem.def );
                // Lower the Better
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, true );
                y += LabelSpacing;

                PrintStatName( "Cooldown" );
                activeValue = Equipment_Ranged_Calculators.Cooldown( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.Cooldown( ProjectedItem );
                // Lower the Better
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, true );
                y += LabelSpacing;

                PrintStatName( "BurstShotCount" );
                activeValue = Equipment_Ranged_Calculators.BurstShotCount( ActiveWeapon[ 0 ].def );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.BurstShotCount( ProjectedItem.def );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Average DPS Short Range" );
                activeValue = Equipment_Ranged_Calculators.AverageShortDPS( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AverageShortDPS( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Average DPS Medium Range" );
                activeValue = Equipment_Ranged_Calculators.AverageMediumDPS( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AverageMediumDPS( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Average DPS Long Range" );
                activeValue = Equipment_Ranged_Calculators.AverageLongDPS( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AverageLongDPS( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Average Damage Per Second" );
                activeValue = Equipment_Ranged_Calculators.AverageDPS( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AverageDPS( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Accuracy Touch" );
                activeValue = Equipment_Ranged_Calculators.AccuracyTouch( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue, true );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AccuracyTouch( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, false, true );
                y += LabelSpacing;

                PrintStatName( "Accuracy Short" );
                activeValue = Equipment_Ranged_Calculators.AccuracyShort( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue, true );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AccuracyShort( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, false, true );
                y += LabelSpacing;

                PrintStatName( "Accuracy Medium" );
                activeValue = Equipment_Ranged_Calculators.AccuracyMedium( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue, true );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AccuracyMedium( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, false, true );
                y += LabelSpacing;

                PrintStatName( "Accuracy Long" );
                activeValue = Equipment_Ranged_Calculators.AccuracyLong( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue, true );
                if( isProjected )
                    projValue = Equipment_Ranged_Calculators.AccuracyLong( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue, false, true );
                y += LabelSpacing;
            }
            else
            {
                PrintStatName( "Damage" );
                activeValue = Equipment_Melee_Calculators.Damage( ActivePawn, ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Melee_Calculators.Damage( ActivePawn, ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;

                PrintStatName( "Warmup" );
                activeValue = Equipment_Melee_Calculators.Warmup( ActiveWeapon[ 0 ].def );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Melee_Calculators.Warmup( ProjectedItem.def );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );

                y += LabelSpacing;

                PrintStatName(  "Cooldown" );
                activeValue = Equipment_Melee_Calculators.Cooldown( ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Melee_Calculators.Cooldown( ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );

                y += LabelSpacing;

                PrintStatName( "Damage Per Second" );
                activeValue = Equipment_Melee_Calculators.DPS( ActivePawn, ActiveWeapon[ 0 ] );
                PrintActiveStat( PrintRectActiveOffensive(), activeValue );
                if( isProjected )
                    projValue = Equipment_Melee_Calculators.DPS( ActivePawn, ProjectedItem );
                PrintProjectedStat( PrintRectProjOffensive(), activeValue, projValue );
                y += LabelSpacing;
            }
 
            SetInitialWindowSettings();
        }

        private void DrawThingRow( ref float y, float width, Thing thing, bool showDropButtonIfPrisoner = false )
        {
            //Rect rect = new Rect( 0f, y, width, 28f );
            //Widgets.InfoCardButton( rect.width - 24f, y, thing );
            //rect.width -= 24f;
            //if( ( this.SelPawnForGear.Faction == Faction.OfPlayer && this.SelPawnForGear.RaceProps.packAnimal ) || ( showDropButtonIfPrisoner && this.SelPawnForGear.HostFaction == Faction.OfPlayer ) )
            //{
            //    Rect rect2 = new Rect( rect.width - 24f, y, 24f, 24f );
            //    TooltipHandler.TipRegion( rect2, "DropThing".Translate() );
            //    if( Widgets.ButtonImage( rect2, TexButton.Drop ) )
            //    {
            //        SoundDefOf.TickHigh.PlayOneShotOnCamera();
            //        this.InterfaceDrop( thing );
            //    }
            //    rect.width -= 24f;
            //}
            //if( this.CanControl && thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && base.SelPawn.RaceProps.CanEverEat( thing ) )
            //{
            //    Rect rect3 = new Rect( rect.width - 24f, y, 24f, 24f );
            //    TooltipHandler.TipRegion( rect3, "ConsumeThing".Translate( new object[]
            //    {
            //        thing.LabelNoCount
            //    } ) );
            //    if( Widgets.ButtonImage( rect3, TexButton.Ingest ) )
            //    {
            //        SoundDefOf.TickHigh.PlayOneShotOnCamera();
            //        this.InterfaceIngest( thing );
            //    }
            //    rect.width -= 24f;
            //}
            //if( Mouse.IsOver( rect ) )
            //{
            //    GUI.color = ITab_Pawn_Gear.HighlightColor;
            //    GUI.DrawTexture( rect, TexUI.HighlightTex );
            //}
            //if( thing.def.DrawMatSingle != null && thing.def.DrawMatSingle.mainTexture != null )
            //{
            //    Widgets.ThingIcon( new Rect( 4f, y, 28f, 28f ), thing, 1f );
            //}
            //Text.Anchor = TextAnchor.MiddleLeft;
            //GUI.color = ITab_Pawn_Gear.ThingLabelColor;
            //Rect rect4 = new Rect( 36f, y, width - 36f, 28f );
            //string text = thing.LabelCap;
            //Apparel apparel = thing as Apparel;
            //if( apparel != null && this.SelPawnForGear.outfits != null && this.SelPawnForGear.outfits.forcedHandler.IsForced( apparel ) )
            //{
            //    text = text + ", " + "ApparelForcedLower".Translate();
            //}
            //Widgets.Label( rect4, text );
            //y += 28f;
        }

        public void DrawColonist( Rect rect, Pawn colonist, Map pawnMap )
        {
            GUI.DrawTexture( this.GetPawnTextureRect( rect.x, rect.y ), PortraitsCache.Get( colonist, ColonistBarColonistDrawer.PawnTextureSize, new Vector3( 0f, 0f, 0.3f ), 1.28205f ) );
            GUI.color = new Color( 1f, 1f, 1f, 1f );
            //this.DrawIcons( rect, colonist );
        }

        private Rect GetPawnTextureRect( float x, float y )
        {
            Vector2 vector = new Vector2( ColonistRect.width / 2, ColonistRect.height / 2);
            Rect rect = new Rect( x + 1f, y - 1f, vector.x, vector.y );
            rect = rect.ContractedBy( 1f );
            return rect;
        }

        protected void BuildColonistButton()
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach( var p in pawns )
            {
                options.Add( new FloatMenuOption( p.NameStringShort, delegate
                {
                    ActivePawn = p;
                } ) );
            }
            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        protected override void DrawPawnRow( Rect rect, Pawn p )
        {
        }

    }
}