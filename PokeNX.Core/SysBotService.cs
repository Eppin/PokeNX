namespace PokeNX.Core;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Models.Enums;
using Utils;
using static Models.Enums.SwitchOffset;

public abstract class SysBotService
{
    private const int MaximumTransferSize = 0x1C0;
    private const int BaseDelay = 1;
    private const int DelayFactor = 1000;

    private readonly Socket _connection = new(SocketType.Stream, ProtocolType.Tcp);
    private readonly object _lock = new();

    protected bool Connected
    {
        get
        {
            lock (_lock)
            {
                return _connection.Connected;
            }
        }
    }

    protected void Connect(IPAddress address, int port)
    {
        lock (_lock)
        {
            _connection.Connect(address, port);
        }
    }

    public void Disconnect()
    {
        if (!Connected)
            return;

        lock (_lock)
        {
            _connection.Disconnect(false);
        }
    }

    public string GetTitleId()
    {
        var bytes = ReadRaw(SwitchCommand.GetTitleId(), 17);

        return Encoding.ASCII.GetString(bytes).Trim();
    }

    public string GetBuildId()
    {
        var bytes = ReadRaw(SwitchCommand.GetBuildId(), 17);

        return Encoding.ASCII.GetString(bytes).Trim();
    }

    public byte[] ReadBytesMain(ulong offset, int length) => ReadBytes(offset, length, Main);
    public byte[] ReadBytesAbsolute(ulong offset, int length) => ReadBytes(offset, length, Absolute);

    public byte[] ReadPointer(string pointer, ushort size)
    {
        lock (_lock)
        {
            _connection.Send(SwitchCommand.PointerPeek(pointer, size));

            // Give it time to push data back
            Thread.Sleep(size / 256 + 100);

            var buffer = new byte[size * 2 + 1];
            ReadInternal(buffer);

            return buffer.ConvertHexBytes();
        }
    }

    private int ReadInternal(byte[] buffer)
    {
        var bytesReceived = _connection.Receive(buffer, 0, 1, SocketFlags.None);

        while (buffer[bytesReceived - 1] != (byte)'\n')
            bytesReceived += _connection.Receive(buffer, bytesReceived, 1, SocketFlags.None);

        return bytesReceived;
    }

    private byte[] ReadBytes(ulong offset, int length, SwitchOffset type)
    {
        lock (_lock)
        {
            var method = type.GetReadMethod();
            _connection.Send(method(offset, length));

            // give it time to push data back
            Thread.Sleep(length / 256 + 100);

            var buffer = new byte[length * 2 + 1];
            ReadInternal(buffer);

            return buffer.ConvertHexBytes();
        }
    }

    private byte[] ReadRaw(byte[] command, int length)
    {
        lock (_lock)
        {
            _connection.Send(command);

            var buffer = new byte[length];
            ReadInternal(buffer);

            return buffer;
        }
    }

    public void WriteBytesMain(ulong offset, byte[] data) => WriteBytes(offset, data, Main);
    public void WriteBytesHeap(ulong offset, byte[] data) => WriteBytes(offset, data, Heap);
    public void WriteBytesAbsolute(ulong offset, byte[] data) => WriteBytes(offset, data, Absolute);

    private void WriteBytes(ulong offset, byte[] data, SwitchOffset type)
    {
        lock (_lock)
        {
            var method = type.GetWriteMethod();
            if (data.Length <= MaximumTransferSize)
            {
                _connection.Send(method(offset, data));
                return;
            }

            for (var i = 0; i < data.Length; i += MaximumTransferSize)
            {
                var slice = data.SliceSafe(i, MaximumTransferSize);
                _connection.Send(method(offset + (uint)i, slice));

                Thread.Sleep((MaximumTransferSize / DelayFactor) + BaseDelay);
            }
        }
    }


    protected async Task Click(SwitchButton button, int delay, CancellationToken token = default)
    {
        lock (_lock)
        {
            _connection.Send(SwitchCommand.Click(button));
        }

        await Task.Delay(delay, token).ConfigureAwait(false);
    }


    protected async Task SetStick(SwitchStick stick, short x, short y, int delay, CancellationToken token = default)
    {
        lock (_lock)
        {
            _connection.Send(SwitchCommand.SetStick(stick, x, y));
        }

        await Task.Delay(delay, token).ConfigureAwait(false);
    }

    protected Task ResetStick(SwitchStick stick, int delay, CancellationToken token = default) => SetStick(stick, 0, 0, delay, token);
}
