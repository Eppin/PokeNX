namespace PokeNX.Core.Utils;

using System.Linq;
using System.Text;
using Models.Enums;

internal static class SwitchCommand
{
    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Address of the data</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Peek(uint offset, int count) => Encode($"peek 0x{offset:X8} {count}");

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from absolute <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Absolute address of the data</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekAbsolute(ulong offset, int count) => Encode($"peekAbsolute 0x{offset:X16} {count}");

    /// <summary>
    /// Requests the Bot to send <see cref="count"/> bytes from main <see cref="offset"/>.
    /// </summary>
    /// <param name="offset">Address of the data relative to main</param>
    /// <param name="count">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PeekMain(ulong offset, int count) => Encode($"peekMain 0x{offset:X16} {count}");

    /// <summary>
    /// Retrieves <see cref="size"/> bytes from pointer <see cref="pointer"/>
    /// </summary>
    /// <param name="pointer">Pointer address</param>
    /// <param name="size">Amount of bytes</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] PointerPeek(string pointer, ushort size)
    {
        var jumps = pointer
            .Replace("[", "")
            .Replace("main", "")
            .Split(']')
            .Select(j => j.Replace("+", ""));

        return Encode($"pointerPeek 0x{size:X} 0x{string.Join(" 0x", jumps)}");
    }

    /// <summary>
    /// Requests the title ID of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetTitleId() => Encode("getTitleID");

    /// <summary>
    /// Requests the build ID of attached process.
    /// </summary>
    /// <returns>Encoded command bytes</returns>
    public static byte[] GetBuildId() => Encode("getBuildID");

    /// <summary>
    /// Presses and releases a <see cref="SwitchButton"/> for 50ms.
    /// </summary>
    /// <remarks>Press &amp; Release timing is performed by the console automatically.</remarks>
    /// <param name="button">Button to click.</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] Click(SwitchButton button) => Encode($"click {button}");

    /// <summary>
    /// Sets the specified <see cref="stick"/> to the desired <see cref="x"/> and <see cref="y"/> positions.
    /// </summary>
    /// <param name="stick">Stick to reset</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] SetStick(SwitchStick stick, short x, short y) => Encode($"setStick {stick} {x} {y}");

    /// <summary>
    /// Resets the specified <see cref="stick"/> to (0,0)
    /// </summary>
    /// <param name="stick">Stick to reset</param>
    /// <returns>Encoded command bytes</returns>
    public static byte[] ResetStick(SwitchStick stick) => SetStick(stick, 0, 0);

    private static byte[] Encode(string command) => Encoding.UTF8.GetBytes(command + "\r\n");
}
