using Microsoft.AspNetCore.Mvc;
using YDotNet.Server.WebSockets;

namespace Demo.Controllers;

public class CollaborationController : Controller
{
    [HttpGet("/collaboration2/{roomName}")]
    public IActionResult Room(string roomName)
    {
        return new YDotNetActionResult(roomName);
    }
}
