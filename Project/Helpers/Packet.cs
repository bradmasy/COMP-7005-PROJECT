using System.Net;
using System.Text;
using static Helpers.Constants;

namespace Helpers;

/**
 * Represents a packet to our protocol
 *
 * The Protocol expected in data transfer is the following
 *
 * The size: The size of the entire payload the
 */
public class Packet : IComparable
{
    /**
     * The sequence number representing the ordering of packets
     *
     */
    public required int SequenceNumber { get; init; }

    /**
     * The acknowledgement number, a number used to identify the order of the packets so that
     * a receiver may order their incoming transmission properly
     *
     * when an acknowledgement number is 0 it is the first packet in communication.
     */
    public required int AckNumber { get; init; }

    public required string Payload { get; init; }
    public int Port { get; set; } = 0;
    public IPAddress? EndPoint { get; set; }


    /**
     * Converts the packet into its header format in bytes
     */
    public byte[] ConvertPacketToBytes()
    {
        var sequenceNumberBytes = BitConverter.GetBytes(SequenceNumber);
        var ackNumberBytes = BitConverter.GetBytes(AckNumber);
        var payloadBytes = Encoding.UTF8.GetBytes(Payload);
        var portBytes = BitConverter.GetBytes(Port);
        var endPointBytes = EndPoint?.GetAddressBytes();

        // create a packet of dynamic length where the header size is fixed but payload is n bytes
        var packet = new byte[HeaderSize + payloadBytes.Length];

        if (endPointBytes != null) Buffer.BlockCopy(endPointBytes, OffSet, packet, 0, IpSize);
        Buffer.BlockCopy(portBytes, OffSet, packet, IpSize, PortSize);
        Buffer.BlockCopy(sequenceNumberBytes, OffSet, packet, IpSize + PortSize, SequenceNumberSize);
        Buffer.BlockCopy(ackNumberBytes, OffSet, packet, IpSize + PortSize + SequenceNumberSize, AckNumberSize);
        Buffer.BlockCopy(payloadBytes, OffSet, packet, HeaderSize,
            payloadBytes.Length);
        return packet;
    }

    public static Packet ConvertBytesToPacket(byte[] buffer)
    {
        var endPoint = new IPAddress(new ReadOnlySpan<byte>(buffer, 0, IpSize));
        var port = BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer, IpSize, PortSize));
        var sequenceNumber =
            BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer, IpSize + PortSize, SequenceNumberSize));
        var ackNumber =
            BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer, IpSize + PortSize + SequenceNumberSize, AckNumberSize));

        var payload = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(buffer,
            IpSize + PortSize + SequenceNumberSize + AckNumberSize,
            buffer.Length - HeaderSize));


        return new Packet
        {
            EndPoint = endPoint, Port = port, SequenceNumber = sequenceNumber, AckNumber = ackNumber,
            Payload = payload
        };
    }

    public override string ToString()
    {
        return
            $"--Packet--\nEndPoint: {EndPoint}\nPort: {Port}\nSequenceNumber: {SequenceNumber}\nAckNumber: {AckNumber}\nPayload: {Payload}\n";
    }


    public int CompareTo(object obj)
    {
        var otherPacket = obj as Packet;
        if (otherPacket == null) throw new ArgumentException("Object is not a Packet");

        var sequenceComparison = SequenceNumber.CompareTo(otherPacket.SequenceNumber);
        if (sequenceComparison != 0) return sequenceComparison;

        var ackComparison = AckNumber.CompareTo(otherPacket.AckNumber);
        if (ackComparison != 0) return ackComparison;

        var payloadComparison = string.Compare(Payload, otherPacket.Payload, StringComparison.Ordinal);
        return payloadComparison != 0
            ? payloadComparison
            : string.Compare(EndPoint?.ToString(), otherPacket.EndPoint?.ToString(), StringComparison.Ordinal);
    }
}