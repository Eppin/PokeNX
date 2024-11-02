namespace PokeNX.Core.Extensions;

using System;

public static class ByteExtensions
{
    private static bool IsNum(char c) => (uint)(c - '0') <= 9;
    private static bool IsHexUpper(char c) => (uint)(c - 'A') <= 5;

    public static byte[] ConvertHexBytes(this byte[] bytes)
    {
        var result = new byte[bytes.Length / 2];

        for (var i = 0; i < result.Length; i++)
        {
            var _0 = (char)bytes[i * 2 + 0];
            var _1 = (char)bytes[i * 2 + 1];
            result[i] = DecodeTuple(_0, _1);
        }

        return result;
    }

    public static byte[] SliceSafe(this byte[] src, int offset, int length)
    {
        var delta = src.Length - offset;
        if (delta < length)
            length = delta;

        var data = new byte[length];
        Buffer.BlockCopy(src, offset, data, 0, data.Length);

        return data;
    }

    private static byte DecodeTuple(char _0, char _1)
    {
        byte result;
        if (IsNum(_0))
            result = (byte)((_0 - '0') << 4);
        else if (IsHexUpper(_0))
            result = (byte)((_0 - 'A' + 10) << 4);
        else
            throw new ArgumentOutOfRangeException(nameof(_0));

        if (IsNum(_1))
            result |= (byte)(_1 - '0');
        else if (IsHexUpper(_1))
            result |= (byte)(_1 - 'A' + 10);
        else
            throw new ArgumentOutOfRangeException(nameof(_1));

        return result;
    }
}
