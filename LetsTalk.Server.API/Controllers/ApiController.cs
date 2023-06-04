using LetsTalk.Server.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[ApiController]
[Authorize]
public class ApiController : ControllerBase
{
    protected int GetAccountId()
    {
        return (int)HttpContext.Items["AccountId"]!;
    }
}
