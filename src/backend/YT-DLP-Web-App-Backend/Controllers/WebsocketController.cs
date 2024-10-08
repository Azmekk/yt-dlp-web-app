﻿using Microsoft.AspNetCore.Mvc;
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

                OnVideoDownloadUpdated videoDownloadHandler = (VideoDownloadInfo videoInfo) =>
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
                
                OnMp3Converted mp3ConversionHandler = (Mp3ConvertedInfo mp3ConversionInfo) =>
                {
                    if(webSocket.State == WebSocketState.Open)
                    {
                        //Wrapper because otherwise messages get polled for some reason.
                        _ = Task.Run(async () =>
                        {
                            await webSocket.SendAsync(
                                Encoding.ASCII.GetBytes(JsonSerializer.Serialize(mp3ConversionInfo)),
                                WebSocketMessageType.Text,
                                endOfMessage: true,
                                cancellationToken: default);
                        });
                    }
                };

                MediaInProgressStorage.AddVideoDownloadUpdatedHandler(videoDownloadHandler);
                MediaInProgressStorage.AddMp3ConversionUpdateHandler(mp3ConversionHandler);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

                while(webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                {
                    var receivedBytes = new byte[1024];
                    try
                    {
                        await webSocket.ReceiveAsync(receivedBytes, cts.Token);
                    }
                    catch(Exception)
                    {
                        break;
                    }

                    if(receivedBytes.Length > 0)
                    {
                        string message = Encoding.UTF8.GetString(receivedBytes);
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
                
                MediaInProgressStorage.RemoveVideoDownloadUpdatedHandler(videoDownloadHandler);
                MediaInProgressStorage.RemoveM3ConversionUpdateHandler(mp3ConversionHandler);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
