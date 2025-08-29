using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Network;

namespace SandboxTesting.Client
{
    public class HeadlessClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private NetState _netState;

        public bool Connected => _client?.Connected ?? false;

        public async Task<bool> AuthenticateAsync(IPAddress address, int port, string username, string password)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(address, port).ConfigureAwait(false);

            _stream = _client.GetStream();

            var seed = new byte[4];
            await _stream.WriteAsync(seed, 0, seed.Length).ConfigureAwait(false);

            _netState = new NetState(new SocketState(_client.Client, seed));

            var pw = new PacketWriter(62);
            pw.Write((byte)0x80);
            pw.WriteAsciiFixed(username, 30);
            pw.WriteAsciiFixed(password, 30);
            pw.Write((byte)0);
            var packet = pw.ToArray();
            await _stream.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);

            var buffer = new byte[1024];
            var bytes = await _stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            var reader = new PacketReader(buffer, bytes, true);
            var id = reader.ReadByte();

            return id == 0xA8;
        }

        public async Task SendPacketAsync(byte[] packet)
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Not connected");
            }

            await _stream.WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
        }

        public async Task<byte[]> ReceivePacketAsync(int length)
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("Not connected");
            }

            var buffer = new byte[length];
            var read = 0;

            while (read < length)
            {
                var n = await _stream.ReadAsync(buffer, read, length - read).ConfigureAwait(false);
                if (n <= 0)
                {
                    throw new InvalidOperationException("Connection closed");
                }

                read += n;
            }

            return buffer;
        }

        public async Task<byte[]> ReceivePacketAsync()
        {
            var header = await ReceivePacketAsync(3).ConfigureAwait(false);
            var length = (header[1] << 8) | header[2];

            if (length <= 3)
            {
                return header;
            }

            var payload = await ReceivePacketAsync(length - 3).ConfigureAwait(false);
            var packet = new byte[length];
            Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            Buffer.BlockCopy(payload, 0, packet, 3, payload.Length);
            return packet;
        }

        public async Task<byte> PingAsync(byte sequence)
        {
            var pw = new PacketWriter(2);
            pw.Write((byte)0x73);
            pw.Write(sequence);
            await SendPacketAsync(pw.ToArray()).ConfigureAwait(false);

            var response = await ReceivePacketAsync(2).ConfigureAwait(false);
            var reader = new PacketReader(response, response.Length, true);
            reader.ReadByte();
            return reader.ReadByte();
        }

        public async Task SendClientVersionAsync(string version)
        {
            var length = (ushort)(3 + (version?.Length ?? 0) + 1);
            var pw = new PacketWriter(length);
            pw.Write((byte)0xBD);
            pw.Write(length);
            pw.WriteAsciiNull(version ?? string.Empty);
            await SendPacketAsync(pw.ToArray()).ConfigureAwait(false);
        }

        public async Task SayAsync(string text, MessageType type = MessageType.Regular, int hue = 0, int font = 3)
        {
            var length = (ushort)(9 + (text?.Length ?? 0));
            var pw = new PacketWriter(length);
            pw.Write((byte)0x03);
            pw.Write(length);
            pw.Write((byte)type);
            pw.Write((short)hue);
            pw.Write((short)font);
            pw.WriteAsciiNull(text ?? string.Empty);
            await SendPacketAsync(pw.ToArray()).ConfigureAwait(false);
        }

        public void Disconnect()
        {
            _netState?.Dispose();
            _stream?.Dispose();
            _client?.Close();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}

