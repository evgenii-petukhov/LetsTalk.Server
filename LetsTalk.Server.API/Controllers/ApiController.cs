using LetsTalk.Server.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[ApiController]
[Authorize]
public class ApiController : ControllerBase
{
    protected string GetAccountId()
    {
        return (string)HttpContext.Items["AccountId"]!;
    }
}
