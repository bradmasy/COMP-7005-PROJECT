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
public class Packet
{
    /**
     * The sequence number representing the ordering of packets
     *
     */
    public required int SequenceNumber { get; set; }

    /**
     * The acknowledgement number, a number used to identify the order of the packets so that
     * a receiver may order their incoming transmission properly
     *
     * when an acknowledgement number is 0 it is the first packet in communication.
     */
    public required int AckNumber { get; set; }

    public required string Payload { get; set; }

    /**
     * Converts the packet into its header format in bytes
     */
    public byte[] ConvertPacketToBytes()
    {
        var sequenceNumberBytes = BitConverter.GetBytes(SequenceNumber);
        var ackNumberBytes = BitConverter.GetBytes(AckNumber);
        var payloadBytes = Encoding.UTF8.GetBytes(Payload);

        // create a packet of dynamic lenght where the header size is fixed but payload is n bytes
        var packet = new byte[HeaderSize + payloadBytes.Length];

        Buffer.BlockCopy(sequenceNumberBytes, OffSet, packet, OffSet, SequenceNumberSize);
        Buffer.BlockCopy(ackNumberBytes, OffSet, packet, SequenceNumberSize, AckNumberSize);
        Buffer.BlockCopy(payloadBytes, OffSet, packet, HeaderSize, payloadBytes.Length);

        return packet;
    }
}