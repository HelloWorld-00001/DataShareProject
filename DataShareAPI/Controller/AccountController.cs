using DataShareCore.Dtos;
using DataShareCore.Models;
using DataShareData;
using Microsoft.AspNetCore.Components;
using WebApplication1.Service.AccountService;

namespace WebApplication1.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/account")]
[Authorize]
// inheritance from abtract class
public class AccountController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;

    // init interface for this class
    readonly IAccountService _accountService;
    public AccountController(IAccountService accountService, IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        _accountService = accountService;
    }
    
    [HttpGet("all"), Authorize]
    public async Task<ActionResult<List<Account>>> GetAllAccount()
    {
        var result = await _accountService.GetAllAccount();
        if (result is null)
            return NotFound("Account not found");

        return Ok(result);
    }
   

    [HttpGet("user")]
    public async Task<ActionResult<Account>> GetAccountByUsername([FromQuery]string email)
    {
        var result =  await _accountService.GetByEmail(email);
        if (result is null)
            return NotFound("Account not found");

        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetAccountById([FromRoute] int id)
    {
        var result =  await _accountService.GetById(id);
        if (result is null)
            return NotFound("Account not found");

        return Ok(result);
    }
    
   

    
}