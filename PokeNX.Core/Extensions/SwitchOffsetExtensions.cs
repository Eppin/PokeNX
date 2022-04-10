namespace PokeNX.Core.Extensions;

using System;
using Models.Enums;
using Utils;

public static class SwitchOffsetExtensions
{
    public static Func<ulong, int, byte[]> GetReadMethod(this SwitchOffset type) => type switch
    {
        SwitchOffset.Heap => (offset, size) => SwitchCommand.Peek((uint)offset, size),
        SwitchOffset.Main => SwitchCommand.PeekMain,
        SwitchOffset.Absolute => SwitchCommand.PeekAbsolute,
        _ => throw new IndexOutOfRangeException("Invalid offset type"),
    };
}
