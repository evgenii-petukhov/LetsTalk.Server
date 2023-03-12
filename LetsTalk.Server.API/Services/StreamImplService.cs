using Grpc.Core;
using Grpc.Gateway.Testing;

namespace LetsTalk.Server.API.Services;

public class StreamImplService : EchoService.EchoServiceBase
{

    private readonly List<string> _messages = new()
    {
      "Hello",
      "World",
      "!"
    };

    public override async Task ServerStreamingEcho(ServerStreamingEchoRequest request, IServerStreamWriter<ServerStreamingEchoResponse> responseStream, ServerCallContext context)
    {
        
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                foreach (var message in _messages)
                {
                    await responseStream.WriteAsync(new ServerStreamingEchoResponse
                    {
                        Message = message
                    });

                    Thread.Sleep(3000);
                }

            }
            catch { }
        }
    }
}