using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using ClashLand.Core;
using ClashLand.Core.Networking;
using ClashLand.Extensions;
using ClashLand.Extensions.Binary;
using ClashLand.Logic.Enums;
using ClashLand.Packets;
using ClashLand.Packets.Cryptography;
using ClashLand.Packets.Cryptography.RC4;
using ClashLand.Packets.Messages.Server;
using System.Net;
using System.Text;

namespace ClashLand.Logic
{
    internal partial class Device
    {
        internal Level Player;
        internal static List<Level> m_vOnlinePlayers = new List<Level>();
        internal string AndroidID, OpenUDID, Model, OSVersion, MACAddress, AdvertiseID, VendorID, IPAddress;
        internal bool Android, Advertising;
        internal int Last_Checksum, Last_Tick, Depth;


        internal Crypto Keys { get; }
        internal Socket Socket { get; }
        internal IntPtr SocketHandle { get; }
        internal List<byte> Stream { get; }
        internal State State = State.DISCONNECTED;
        internal object GameMode;

        internal uint ClientSeed { get; set; }

        internal byte[] IncomingPacketsKey { get; set; }
        internal byte[] OutgoingPacketsKey { get; set; }

        internal DateTime NextKeepAlive { get; set; }
        internal DateTime LastKeepAlive { get; set; }

        internal readonly Keep_Alive_OK Keep_Alive;


        public Device(Socket so)
        {
            this.Socket = so;
            if (Constants.RC4)
            {
                IncomingPacketsKey = new byte[Key._RC4_EndecryptKey.Length];
                Array.Copy(Key._RC4_EndecryptKey, IncomingPacketsKey, Key._RC4_EndecryptKey.Length);

                OutgoingPacketsKey = new byte[Key._RC4_EndecryptKey.Length];
                Array.Copy(Key._RC4_EndecryptKey, OutgoingPacketsKey, Key._RC4_EndecryptKey.Length);
            }
            else
            {
                this.Keys = new Crypto();
            }

            this.SocketHandle = so.Handle;

            this.IPAddress = ((IPEndPoint) so.RemoteEndPoint).Address.ToString();
            this.Keep_Alive = new Keep_Alive_OK(this);
            this.LastKeepAlive = DateTime.Now;
            this.NextKeepAlive = this.LastKeepAlive.AddSeconds(30);
            this.Stream = new List<byte>(Constants.Buffer);
        }

        internal bool TryGetPacket(out Message message)
        {
            const int HEADER_LEN = 7;
            message = default(Message);
            var result = false;
            if (Stream.Count >= 5)
            {
                int length = (Stream[2] << 16) | (Stream[3] << 8) | Stream[4];
                ushort type = (ushort)((Stream[0] << 8) | Stream[1]);

                if (Stream.Count - HEADER_LEN >= length)
                {                    // Avoid LINQ cause we can.
                    var packet = new byte[length];
                    for (int i = 0; i < packet.Length; i++)
                    {
                        packet[i] = Stream[i + HEADER_LEN];
                    }

                    message = MessageFactory.Parse(this, type);
                    if (message != null)
                    {
                        message.Data = new List<byte>(packet);

                        message.Identifier = type;
                        try
                        {
                            if (Constants.RC4)
                            message.Decrypt();
                            else
                            message.DecryptSexy();
                        }
                        catch (Exception ex)
                        {
                            //ExceptionLogger.Log(ex, $"Unable to decrypt message with ID: {type}");
                        }

                        try
                        {
                            message.Decode();
                        }
                        catch (Exception ex)
                        {
                            //ExceptionLogger.Log(ex, $"Unable to decode message with ID: {type}");
                        }
#if DEBUG
                        //Control.Say(message.Device.Socket.RemoteEndPoint + " --> " + message.GetType().Name);
#endif
                        result = true;
                    }
                    else
                    {
                        Console.WriteLine("Unhandled message " + type);
                        //Logger.Say("Unhandled message " + type);

                        // Make sure we don't break the RC4 stream.
                        this.Decrypt(packet.ToArray());
                        packet = null;
                    }
                    Stream.RemoveRange(0, HEADER_LEN + length);
                }
            }
            return result;
        }
    }
}