namespace Helpers;

public static class Constants
{
    // Server Constants
    public const int MinPort = 0;
    public const int MaxPort = 65535;
    public const int ServerIp = 0;
    public const int ServerPort = 1;
    public const int Connections = 10;
    
    // Packet Constants
    
    // Sizes
    private const int Bit = 1;
    private const int Byte = Bit * 4;
    private const int Chunk = 1024;
    public const int PacketSize = HeaderSize + Chunk;
    public const int InitiateSequenceNumber = -1;
        
    // the max size of the header is 32-Bits or 4 Bytes
    public const int MinHeaderSize = Byte * 2;
    public const int MaxHeaderSize = Byte * 4;
    public const int OffsetSize = Bit * 4;
    public const int OffSet = 0;
    
    public const int PortSize = Bit * 2;
    public const int IpSize = Bit * 4;
    public const int SequenceNumberSize = Bit * 4;
    public const int AckNumberSize = Bit * 4;
    public const int HeaderSize = IpSize + PortSize + SequenceNumberSize + AckNumberSize;
    
    // Client Constants

    public const int TargetIp = 0;
    public const int TargetPort = 1;
    public const int TimeoutArg = 2;
    public const int MaxRetry = 3;
    public const int ValidClientArgs = 4;

    // Proxy

    public const int ValidProxyArgs = 12;
    public const int ListenIp = 0;
    public const int ListenPort = 1;
    public const int ForwardingIp = 2;
    public const int ForwardingPort = 3;
    public const int ClientDropPercentage = 4;
    public const int ServerDropPercentage = 5;
    public const int ClientDelayChancePercentage = 6;
    public const int ServerDelayChancePercentage = 7;
    public const int ClientDelayTimeMin = 8;
    public const int ClientDelayTimeMax = 9;
    public const int ServerDelayTimeMin = 10;
    public const int ServerDelayTimeMax = 11;
    
    // UDP
    public const int MaxPacketSize = 65535;
}