using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum GESTURE
{
    NICHTS = -1,
    GEIGE = 0,
    TROMMEL = 1,
    TROMPETE = 2
};

public class gestureList
{
    /// <summary> Name of the discrete gestures in the database that we want to track </summary>
    public static readonly string[] gestures = { "violin", "drums", "trumpet"};
}
