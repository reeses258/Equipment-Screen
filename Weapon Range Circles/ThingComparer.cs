using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Linq;

namespace WRS
{
    class ThingComparer : IEqualityComparer<Thing>
    {
        public bool Equals( Thing t1, Thing t2 )
        {
            if( t1.def.defName != t2.def.defName )
            {
                return false;
            }

            var quality1 = QualityCategory.Awful;
            var quality2 = QualityCategory.Awful;
            if( t1.TryGetQuality( out quality1 ) != t2.TryGetQuality( out quality2 ) )
            {
                return false;
            }
            return true;
        }
        public int GetHashCode( Thing t1 )
        {
            return t1.GetHashCode();
        }
    }
}
