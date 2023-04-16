using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected int GetAccountId()
    {
        return (int)HttpContext.Items["AccountId"]!;
    }
}
