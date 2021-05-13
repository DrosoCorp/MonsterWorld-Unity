using MonsterWorld.Unity.Network;
using System;
using System.Collections.Generic;
using System.IO;

public class NetworkManager
{
    // Handles network messages on client and server
    protected delegate void NetworkMessageDelegate(int connectionID);

    /// <summary>
    /// The registered network message handlers.
    /// </summary>
    static readonly Dictionary<byte, NetworkMessageDelegate> handlers = new Dictionary<byte, NetworkMessageDelegate>();
    static readonly Dictionary<byte, MemoryStream> memories = new Dictionary<byte, MemoryStream>();

    //Send packet
    static protected ArraySegment<byte> GetBytesFromPacket(IPacket packet)
    {
        MemoryStream stream = new MemoryStream(512);
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(packet.OpCode);
        packet.Serialize(writer);
        stream.TryGetBuffer(out ArraySegment<byte> segment);
        return segment;
    }

    /// <summary>
    /// This function register an handler for a packet
    /// </summary>
    public static void RegisterHandler<T>(Action<T, int> handler) where T : struct, IPacket
    {
        NetworkMessageDelegate del = (connectionID) =>
        {
            T packet = default;
            packet.Deserialize(new BinaryReader(memories[packet.OpCode]));
            handler(packet, connectionID);
        };
        T p = default;
        handlers.Add(p.OpCode, del);
        int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        memories.Add(p.OpCode, new MemoryStream(size));
    }

    protected static void HandlePacket(ArraySegment<byte> bytes, int connectionID)
    {
        byte op = bytes.Array[0];
        byte[] buffer = memories[op].GetBuffer();
        System.Buffer.BlockCopy(bytes.Array, bytes.Offset + 1, buffer, 0, bytes.Count - 1);
        handlers[op](connectionID);
    }

}
