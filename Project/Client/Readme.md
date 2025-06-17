# UDP Client Application

This is a .NET-based UDP client application that implements reliable packet transmission with acknowledgment and retry mechanisms.

## Project Structure
```
├── Client/                 # Client application
│   ├── Client.cs          # Main client implementation with reliability features
│   └── Program.cs         # Client entry point
└── Helpers/               # Shared helper classes
    ├── Packet.cs          # Packet structure and serialization
    └── Constants.cs       # Application constants
```

## Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code with C# extensions

## Building the Project
```bash
# Navigate to the Client directory
cd Client

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

## Running the Application

### Starting the Client
Navigate to the Client directory and run:

```bash
dotnet run <server_ip> <server_port> <timeout> <max_retries>
```

#### Arguments:
- `server_ip`: The IP address of the server to connect to (e.g., 127.0.0.1)
- `server_port`: The port number of the server (e.g., 80)
- `timeout`: Timeout in seconds for packet acknowledgment (e.g., 3)
- `max_retries`: Maximum number of retry attempts (e.g., 5)

#### Example:
```bash
dotnet run 127.0.0.1 80 3 5
```

## Features

### Reliability Mechanisms
- **Sequence Numbers**: Each packet is assigned a unique sequence number
- **Acknowledgment Tracking**: Client waits for acknowledgment from server
- **Automatic Retransmission**: Retries sending packets if no acknowledgment received
- **Timeout Handling**: Configurable timeout periods for packet acknowledgment
- **Maximum Retries**: Configurable retry limit with error reporting

### Packet Validation
- **Acknowledgment Number Verification**: Ensures received packets match expected sequence numbers
- **Duplicate Packet Handling**: Properly handles duplicate packets and acknowledgments
- **Error Reporting**: Detailed logging of packet mismatches and transmission errors

## Usage Instructions

1. **Start the Server**: Ensure the UDP server is running on the specified IP and port
2. **Run the Client**: Execute the client with appropriate parameters
3. **Enter Messages**: Type payload messages when prompted
4. **Monitor Output**: Watch for acknowledgment confirmations and error messages

## Expected Behavior

- Client prompts for message input
- Sends packets with sequence numbers
- Waits for acknowledgments with timeout
- Retries failed transmissions up to max_retries
- Displays received packets and acknowledgment status
- Continues accepting new messages until manually stopped

## Error Handling

The client handles various error scenarios:
- **Network Timeouts**: Automatic retry with configurable timeout
- **Packet Loss**: Retransmission of unacknowledged packets
- **Invalid Acknowledgments**: Logging and skipping of mismatched packets
- **Socket Errors**: Graceful error reporting and recovery

## Development

The project uses:
- **C# 8.0+** with modern language features
- **Async/Await** for non-blocking I/O operations
- **UDP Sockets** for packet-based communication
- **Task-based Programming** for concurrent operations