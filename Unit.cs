using UnityEngine;
using System;
using System.Collections.Generic;

namespace Barracuda
{
    public class Unit : IComparable<Unit>
    {
        private Unit() {}

        private static Unit @default = new Unit();

        public static Unit Default { get { return @default; } }

        #region IComparable implementation
        public int CompareTo(Unit other)
        {
            return 0;
        }
        #endregion
    }
}
