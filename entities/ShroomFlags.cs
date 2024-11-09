using System;

namespace leafy.entities;

[Flags]
public enum ShroomTraits
{
    Trait1 = 1 << 0,
    Trait2 = 1 << 1,
    Trait3 = 1 << 2,
    Trait4 = 1 << 3,
    Trait5 = 1 << 4,
    Trait6 = 1 << 5,
    Trait7 = 1 << 6,
    Trait8 = 1 << 7,
    Trait9 = 1 << 8,
}
