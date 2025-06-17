# UDP Server Application

This is a .NET-based UDP server application that implements reliable packet reception with sequence number tracking, duplicate detection, and automatic acknowledgment generation.

## Project Structure
```
├── Server/                 # Server application
│   ├── Server.cs          # Main server implementation with reliability features
│   └── Program.cs         # Server entry point
└── Helpers/               # Shared helper classes
    ├── Packet.cs          # Packet structure and serialization
    └── Constants.cs       # Application constants
```

## Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code with C# extensions
- Administrative privileges (for binding to privileged ports)

## Building the Project
```bash
# Navigate to the Server directory
cd Server

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

## Running the Application

### Starting the Server
Navigate to the Server directory and run:

```bash
dotnet run <ip_address> <port>
```

#### Arguments:
- `ip_address`: The IP address to bind the server to (e.g., 127.0.0.1)
- `port`: The port number to listen on (e.g., 80)

#### Examples:

**Standard Operation:**
```bash
dotnet run 127.0.0.1 80
```

**Using Administrative Privileges (for privileged ports):**
```bash
sudo dotnet run 127.0.0.1 80
```

**Custom Network Interface:**
```bash
dotnet run 192.168.1.100 8080
```

## Features

### Reliability Mechanisms
- **Sequence Number Tracking**: Maintains expected sequence numbers for ordered packet processing
- **Duplicate Detection**: Identifies and handles duplicate packets automatically
- **Out-of-Order Handling**: Processes packets in correct sequence order
- **Automatic Acknowledgments**: Generates and sends acknowledgment packets for received data

### Connection Management
- **New Connection Detection**: Automatically detects new client connections
- **Sequence Reset**: Resets sequence tracking for new client sessions
- **Client Tracking**: Maintains separate sequence numbers per client connection
- **Packet History**: Tracks received packets to handle duplicates and retransmissions

### Packet Processing
- **Ordered Delivery**: Ensures packets are processed in sequence order
- **Duplicate Filtering**: Prevents duplicate packet processing
- **Acknowledgment Generation**: Creates proper acknowledgment responses
- **Error Handling**: Graceful handling of malformed or unexpected packets

## Usage Instructions

1. **Configure Server**: Set appropriate IP address and port for binding
2. **Start the Server**: Run the server with desired parameters
3. **Monitor Connections**: Watch console output for incoming connections and packets
4. **Verify Acknowledgments**: Ensure proper acknowledgment generation and sending
5. **Handle Multiple Clients**: Server can handle multiple concurrent client connections

## Expected Behavior

- Server binds to specified IP address and port
- Displays server startup information and listening status
- Accepts incoming UDP packets from clients
- Processes packets in sequence order
- Generates and sends acknowledgment packets
- Handles duplicate and out-of-order packets gracefully
- Displays packet information and acknowledgment status
- Continues listening for new connections

## Packet Processing Flow

1. **Receive Packet**: Accept incoming UDP packet from client
2. **Parse Packet**: Convert received bytes to packet structure
3. **Check Sequence**: Verify packet sequence number against expected value
4. **Handle Duplicates**: Skip processing if packet is duplicate
5. **Process Packet**: Handle valid, in-order packets
6. **Generate ACK**: Create acknowledgment packet with proper sequence/ack numbers
7. **Send Response**: Transmit acknowledgment back to client
8. **Update State**: Increment expected sequence number and track received packet

## Error Handling

The server handles various scenarios:
- **Invalid Packets**: Graceful handling of malformed packet data
- **Network Errors**: Proper socket exception handling
- **Duplicate Packets**: Automatic detection and filtering
- **Out-of-Order Packets**: Sequence number validation and ordering
- **New Connections**: Automatic sequence number reset for new clients

## Connection States

### New Client Connection
- Detects sequence number 0 as new connection indicator
- Resets expected sequence number to 0
- Clears previous packet history
- Logs new connection detection

### Existing Client
- Maintains expected sequence number
- Processes packets in order
- Handles retransmissions and duplicates
- Tracks packet history for validation

## Development

The project uses:
- **C# 8.0+** with modern language features
- **Async/Await** for non-blocking I/O operations
- **UDP Sockets** for packet-based communication
- **SortedDictionary** for efficient packet tracking
- **Sequence Number Management** for reliable packet ordering
