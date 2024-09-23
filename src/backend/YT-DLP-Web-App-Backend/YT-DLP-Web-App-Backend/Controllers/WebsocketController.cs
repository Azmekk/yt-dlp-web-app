using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using YT_DLP_Web_App_Backend.Constants;
using YT_DLP_Web_App_Backend.DataObjects;

namespace YT_DLP_Web_App_Backend.Controllers
{
    public class WebsocketController : Controller
    {
        [Route("/ws")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Get()
        {
            if(HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                OnVideoDownloadUpdated videoDownloadHandler = async (int videoId, VideoDownloadInfo videoInfo) =>
                {
                    if(webSocket.State == WebSocketState.Open)
                    {
                        //Wrapper because otherwise messages get polled for some reason.
                        _ = Task.Run(async () =>
                        {
                            await webSocket.SendAsync(
                                Encoding.ASCII.GetBytes(JsonSerializer.Serialize(videoInfo)),
                                WebSocketMessageType.Text,
                                endOfMessage: true,
                                cancellationToken: default);
                        });
                    }
                };

                VideosInProgressStorage.AddVideoDownloadUpdatedHandler(videoDownloadHandler);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

                while(webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                {
                    var receivedBytes = new byte[1024];
                    try
                    {
                        var result = await webSocket.ReceiveAsync(receivedBytes, cts.Token);
                    }
                    catch(Exception)
                    {
                        break;
                    }

                    if(receivedBytes.Length > 0)
                    {
                        string message = System.Text.Encoding.UTF8.GetString(receivedBytes);
                        if(message.Equals("renew", StringComparison.CurrentCultureIgnoreCase))
                        {
                            cts.CancelAfter(TimeSpan.FromMinutes(5));
                        }
                    }
                }

                if(webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", default);
                }
                
                VideosInProgressStorage.RemoveVideoDownloadUpdatedHandler(videoDownloadHandler);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
