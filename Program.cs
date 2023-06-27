using System.Net.WebSockets;
using Google.Protobuf;
using GrpcServiceWebSocketExample;

class Program
{
    static async Task Main(string[] args)
    {
        using (var webSocket = new ClientWebSocket())
        {
            // Connect to the WebSocket server
            await webSocket.ConnectAsync(new Uri("ws://localhost:5000/"), CancellationToken.None);

            Console.WriteLine("Connected to the WebSocket server.");

            // Create a sample request message
            var requestMessage = new HelloRequest
            {
                Name = $"Client {Guid.NewGuid()}"
            };

            // Serialize the request message to a byte array
            var requestBuffer = requestMessage.ToByteArray();

            // Send the request message to the server
            await webSocket.SendAsync(new ArraySegment<byte>(requestBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);

            // Receive and process the response message
            var responseBuffer = new byte[1024];
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
            if (receiveResult.MessageType == WebSocketMessageType.Binary)
            {
                // Deserialize the response message
                var responseMessage = HelloReply.Parser.ParseFrom(responseBuffer, 0, receiveResult.Count);

                // Process the response message
                Console.WriteLine($"Received response: {responseMessage.Message}");
            }

            // Close the WebSocket connection
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            
            Console.ReadKey();
        }
    }
}