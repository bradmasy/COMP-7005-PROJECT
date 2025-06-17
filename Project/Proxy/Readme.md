# UDP Proxy Server

This is a .NET-based UDP proxy server that acts as an intermediary between clients and servers, providing network simulation capabilities including packet dropping and delay injection.

## Project Structure
```
├── Proxy/                  # Proxy server application
│   ├── Proxy.cs           # Main proxy implementation with network simulation
│   └── Program.cs         # Proxy entry point
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
# Navigate to the Proxy directory
cd Proxy

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

## Running the Application

### Starting the Proxy Server
Navigate to the Proxy directory and run:

```bash
dotnet run <listen_ip> <listen_port> <target_ip> <target_port> <client_drop_percent> <server_drop_percent> <client_delay_percent> <server_delay_percent> <client_delay_min> <client_delay_max> <server_delay_min> <server_delay_max>
```

#### Arguments:
- `listen_ip`: IP address to bind the proxy to (e.g., 127.0.0.1)
- `listen_port`: Port number to listen on (e.g., 8000)
- `target_ip`: IP address of the target server (e.g., 127.0.0.1)
- `target_port`: Port number of the target server (e.g., 80)
- `client_drop_percent`: Percentage of client packets to drop (0-100)
- `server_drop_percent`: Percentage of server packets to drop (0-100)
- `client_delay_percent`: Percentage of client packets to delay (0-100)
- `server_delay_percent`: Percentage of server packets to delay (0-100)
- `client_delay_min`: Minimum delay time for client packets (seconds)
- `client_delay_max`: Maximum delay time for client packets (seconds)
- `server_delay_min`: Minimum delay time for server packets (seconds)
- `server_delay_max`: Maximum delay time for server packets (seconds)

#### Examples:

**Normal Operation (No Packet Loss or Delay):**
```bash
dotnet run 127.0.0.1 8000 127.0.0.1 80 0 0 0 0 0 0 0 0
```

**With Packet Loss and Delay Simulation:**
```bash
dotnet run 127.0.0.1 3000 127.0.0.1 5000 10 10 20 20 5 5 5 5
```

**Using Administrative Privileges (for privileged ports):**
```bash
sudo dotnet run 127.0.0.1 80 127.0.0.1 90 0 0 0 0 0 0 0 0
```

## Features

### Network Simulation
- **Packet Dropping**: Configurable percentage of packets dropped in both directions
- **Packet Delaying**: Configurable percentage of packets delayed with random timing
- **Bidirectional Traffic**: Handles both client-to-server and server-to-client traffic
- **Real-time Monitoring**: Displays incoming traffic and forwarding decisions

### Traffic Management
- **Automatic Routing**: Forwards packets between clients and servers
- **Endpoint Tracking**: Maintains client endpoint information for proper routing
- **Asynchronous Processing**: Handles multiple packets concurrently
- **Buffer Management**: Efficient memory usage with buffer clearing

### Configuration Options
- **Flexible Parameters**: All simulation parameters are configurable at runtime
- **Directional Control**: Separate settings for client and server traffic
- **Time-based Delays**: Random delay times within specified ranges
- **Percentage-based Simulation**: Realistic network condition simulation

## Usage Instructions

1. **Configure Parameters**: Set appropriate IP addresses, ports, and simulation parameters
2. **Start the Proxy**: Run the proxy with desired configuration
3. **Connect Clients**: Direct clients to the proxy's listen address and port
4. **Monitor Traffic**: Watch console output for traffic flow and simulation effects
5. **Verify Routing**: Ensure packets are properly forwarded between clients and servers

## Expected Behavior

- Proxy binds to specified listen address and port
- Displays incoming packet information and source endpoints
- Applies configured drop and delay percentages
- Forwards valid packets to appropriate destinations
- Shows forwarding decisions and simulation effects
- Handles multiple concurrent connections

## Network Simulation Examples

### 10% Drop Rate with 20% Delay Rate
```bash
dotnet run 127.0.0.1 3000 127.0.0.1 5000 10 10 20 20 5 5 5 5
```
This configuration:
- Drops 10% of packets in both directions
- Delays 20% of packets in both directions
- Applies random delays between 5-10 seconds

### High Network Stress Test
```bash
dotnet run 127.0.0.1 8000 127.0.0.1 80 30 30 50 50 10 20 10 20
```
This configuration:
- Drops 30% of packets in both directions
- Delays 50% of packets in both directions
- Applies random delays between 10-20 seconds

## Error Handling

The proxy handles various scenarios:
- **Invalid Parameters**: Validates input parameters and provides error messages
- **Network Errors**: Graceful handling of socket exceptions
- **Memory Management**: Proper buffer clearing to prevent memory leaks
- **Concurrent Access**: Thread-safe packet processing

## Development

The project uses:
- **C# 8.0+** with modern language features
- **Async/Await** for non-blocking I/O operations
- **UDP Sockets** for packet-based communication
- **Task-based Programming** for concurrent packet processing
- **Random Number Generation** for realistic network simulation