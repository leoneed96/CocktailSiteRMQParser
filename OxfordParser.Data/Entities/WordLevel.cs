﻿using System;

namespace OxfordParser.Data.Entities
{
    public enum WordLevel
    {
        None,
        a1,
        a2,
        b1,
        b2,
        c1,
        c2
    }

    [Flags]
    public enum SpecialWordList
    {
        None = 0,
        Oxford3000 = 2,
        Oxford5000 = 4
    }
}
