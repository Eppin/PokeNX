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

    public static Func<ulong, byte[], byte[]> GetWriteMethod(this SwitchOffset type) => type switch
    {
        SwitchOffset.Heap => (offset, bytes) => SwitchCommand.Poke((uint)offset, bytes),
        SwitchOffset.Main => (offset, bytes) => SwitchCommand.PokeMain(offset, bytes),
        SwitchOffset.Absolute => (offset, bytes) => SwitchCommand.PokeAbsolute(offset, bytes),
        _ => throw new IndexOutOfRangeException("Invalid offset type."),
    };

}
