using System;

namespace leafy.entities;

[Flags]
public enum ShroomTraits
{
    Toad = 1 << 0,
    Honey = 1 << 1,
    Porc = 1 << 2,
}
