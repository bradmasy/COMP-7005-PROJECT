# Proxy Server Instructions

## Run the Server:

### Normal
- dotnet run 192.168.1.78 8000 192.168.1.78 90 0 0 0 0 0 0 0 0
- sudo dotnet run 127.0.0.1 8000 127.0.0.1 80 0 0 0 0 0 0 0 0

### 10% Drop Rate with 20% Delay Rate
sudo dotnet run 127.0.0.1 3000 127.0.0.1 5000 10 10 20 20 5 5 5 5