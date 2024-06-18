using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DataShareCore.Common.Helper;
using DataShareCore.Dtos;
using DataShareCore.Mapper;
using DataShareCore.Models;
using DataShareCore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Service.AccountService;
using System.Text;
using System.Web;
using DataShareCore.Common.Enum;
using DataShareData.Repository.AccountRepoFolder;


namespace WebApplication1.Controller;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    readonly IConfiguration _configuration;
    readonly IAccountService _accountService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IConfiguration configuration, IAccountService accService, IRefreshTokenService rts)
    {
        _configuration = configuration;
        _accountService = accService;
        _refreshTokenService = rts;
    }

    [HttpGet]
    public ActionResult<string> GetMe()
    {
       
        return Ok(new APIResponser<string>
        {
            message = "test cookie value",
            content = Request.Cookies[ConfigEnum.RefreshToken]
        });
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<Account>> AddAccount([FromBody] AccountDtos accountDtos)
    {
        var account =  await _accountService.GetByEmail(accountDtos.email);
        if (accountDtos.password.Length < 6)
            return BadRequest("Password is too weak, must be greater than 6 characters");
        if (account is not null)
            return BadRequest("Email already existed");
        
        // encode password
        Account acc = AccountMapper.AccountDtos2Account(accountDtos);
        CreatePasswordHash(accountDtos.password, out byte[] passwordHash, out byte[] passwordSalt);
        // re-assign password
        acc.password = ByteConvertion.ByteArrayToString(passwordHash);
        acc.passwordSalt = ByteConvertion.ByteArrayToString(passwordSalt);
        
        var result =  await _accountService.AddAccount(acc);
        if (result is null)
            return BadRequest("There is something error here");

        var token = GenerateJwtToken(result);
        
        // add new refresh token
        var newRefreshToken = GenerateRefreshToken(new RefreshToken());
        newRefreshToken.userId = result.id;
        
        await _refreshTokenService.AddOne(newRefreshToken);

        // save to cookie
        SetRefreshToken(newRefreshToken);
        
        

        return Ok(result);
    }
    

    [HttpPost("login")]
    public async Task<ActionResult<APIResponser<string>>> Login([FromBody] AccountDtos accountDtos)
    {
        var account =  await _accountService.GetByEmail(accountDtos.email);

        if (account is null) {
            return NotFound("Email not found");
        }

        if (account.passwordSalt.Length < 256)
        {
            return BadRequest("The account is not register by the right way, please try again");
        }
        
        // verify user account
        if ( !VerifyPassword(accountDtos.password, account.password, account.passwordSalt))
        {
            return BadRequest("Wrong credentials");
        }
        
        // access token
        var token = GenerateJwtToken(account);
        var oldToken = await _refreshTokenService.GetByUserId(account.id);

        // update refresh token
        var newRefreshToken = GenerateRefreshToken(oldToken);        
        await _refreshTokenService.UpdateToken(newRefreshToken);

        SetRefreshToken(newRefreshToken);

        return Ok(new APIResponser<Dictionary<String, String>>{
            success = true,
            message = "Refresh successfully",
            content = new Dictionary<string, string> {
                { "accessToken", token },
                { "refreshToken", newRefreshToken.token }
            }
        });
    }
    
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult<APIResponser<Dictionary<String, String>>>> RefreshToken([FromBody] string refreshTokenPara)
    {
        // get user id from jwt token

        int userId = await _refreshTokenService.GetUserIdByToken(refreshTokenPara);
        if (userId < 1)
            return Unauthorized("Invalid refresh token");


        var acc = await _accountService.GetById(userId);
       
        

        // update access token
        string token = GenerateJwtToken(acc);
        var refreshTokenUpdate = await _refreshTokenService.GetByUserId(acc.id);

        // renew refresh token if it about to expire
        if ((refreshTokenUpdate.expiredTime - DateTime.UtcNow).TotalHours < 24 )
        {
            GenerateRefreshToken(refreshTokenUpdate);        
            await _refreshTokenService.UpdateToken(refreshTokenUpdate);

            SetRefreshToken(refreshTokenUpdate);
        }
       
        return Ok(new APIResponser<Dictionary<String, String>>{
            success = true,
            message = "Refresh successfully",
            content = new Dictionary<string, string> {
                { "accessToken", token },
                { "refreshToken", refreshTokenUpdate.token }
            }
        });
    }
    
    [HttpGet("refresh-token-by-cookie")]
    public async Task<ActionResult<APIResponser<Dictionary<String, String>>>> RefreshTokenByCookie()
    {
       
        // auto take refresh toke by cookies
        var refreshToken = "Can not get token from cookie";
        if (Request.Cookies[ConfigEnum.RefreshToken] is not null)
        {
            refreshToken =  Request.Cookies[ConfigEnum.RefreshToken];
        }
        else
        {
            return NotFound("Not found your current token: " + refreshToken);
        }
        
        int userId = await _refreshTokenService.GetUserIdByToken(refreshToken);
        if (userId < 1)
            return Unauthorized("Invalid refresh token");


        var acc = await _accountService.GetById(userId);

        // update access token
        string token = GenerateJwtToken(acc);
        var refreshTokenUpdate = await _refreshTokenService.GetByUserId(acc.id);

        // renew refresh token if it about to expire
        if ((refreshTokenUpdate.expiredTime - DateTime.UtcNow).TotalHours < 24 )
        {
            GenerateRefreshToken(refreshTokenUpdate);        
            await _refreshTokenService.UpdateToken(refreshTokenUpdate);

            SetRefreshToken(refreshTokenUpdate);
        }
       
        return Ok(new APIResponser<Dictionary<String, String>>{
            success = true,
            message = "Refresh successfully",
            content = new Dictionary<string, string> {
                { "accessToken", token },
                { "refreshToken", refreshTokenUpdate.token }
            }
        });
    }

    private RefreshToken GenerateRefreshToken(RefreshToken oldToken)
    {
        oldToken.token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        oldToken.expiredTime = DateTime.UtcNow.AddDays(ConfigEnum.RefreshTokenExpiredDay);
        oldToken.createTime = DateTime.UtcNow;

        return oldToken;
    }

    private void SetRefreshToken(RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.expiredTime,
            Secure = true,
            SameSite = SameSiteMode.None
        };
        
        Response.Cookies.Append(ConfigEnum.RefreshToken, newRefreshToken.token, cookieOptions);

    }

    
    private string GenerateJwtToken(Account acc)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var secretKey = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Secret_Key").Value);

        var tokenDetail = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Email, acc.email),
                new Claim("Id", acc.id.ToString()),
                //skip role
                new Claim("TokenId", Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(ConfigEnum.TokenExpiredMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature)
            
        };
        var token = jwtTokenHandler.CreateToken(tokenDetail);

        return jwtTokenHandler.WriteToken(token);
    }
    

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    protected virtual bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] hashBytes = ByteConvertion.StringToByteArray(storedHash);
        byte[] saltBytes = ByteConvertion.StringToByteArray(storedSalt);

        using (var hmac = new HMACSHA512(saltBytes))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // Compare computed hash with stored hash
            return computedHash.SequenceEqual(hashBytes);
        }
    }


}