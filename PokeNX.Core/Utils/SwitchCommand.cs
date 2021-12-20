namespace PokeNX.Core.Utils
{
    using System.Linq;
    using System.Text;

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
        /// Requests the title id of attached process.
        /// </summary>
        /// <returns>Encoded command bytes</returns>
        public static byte[] GetTitleID() => Encode("getTitleID");

        private static byte[] Encode(string command) => Encoding.UTF8.GetBytes(command + "\r\n");
    }
}
