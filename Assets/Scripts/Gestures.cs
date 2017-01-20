using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public enum GESTURE
    {
        NICHTS = -1,
        GEIGE = 0,
        GITARRE = 1,
        FLOETE = 2,
        HARFE = 3,
        TROMMEL = 4,
        TROMPETE = 5,
        CELLO = 6,
        KLAVIER = 7
    };

    public class gestureList
    {
        /// <summary> Name of the discrete gestures in the database that we want to track </summary>
        public static readonly string[] gestures = { "Geige", "Gitarre", "Querfloete", "Harfe", "Trommel", "Trompete", "Cello", "Klavier" };
    }
}