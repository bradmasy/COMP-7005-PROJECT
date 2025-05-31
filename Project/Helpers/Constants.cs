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
    
    public const int FlagBytes = Bit * 4;
        
    // the max size of the header is 32-Bits or 4 Bytes
    public const int MinHeaderSize = Byte * 2;
    public const int MaxHeaderSize = Byte * 4;
    public const int OffsetSize = Bit * 4;
    public const int OffSet = 0;
    public const int SequenceNumberSize = 4;
    public const int AckNumberSize = 4;
    public const int HeaderSize = SequenceNumberSize + AckNumberSize;
    
    // Client Constants

    public const int TargetIp = 0;
    public const int TargetPort = 1;
    public const int TimeoutArg = 2;
    public const int MaxRetry = 3;

}