using DataShareCore.Common.Enum;
using DataShareCore.Dtos;
using DataShareCore.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Service.AccountService;
using WebApplication1.Service.FileStoreService;
using System.Data;
using System;
using System.Text;
using System.Web;
using DataShareCore.Common.Helper;
using DataShareCore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/message")]
public class TextStoreController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;

    // init interface for this class
    readonly ITextStoreService _textStoreService;

    public TextStoreController(ITextStoreService textStoreService, IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        _textStoreService = textStoreService;
    }
    
    [HttpPost, Authorize]
    public async Task<IActionResult> UploadText([FromForm] TextStoreDtos message)
    {
        
        int userId = JwtHeper.GetUserId(this);

        if (message.content.Length < 1 || message.content.IsNullOrEmpty())
            return BadRequest("No content in your massage");

        var messageContent = new TextStore
        {
            content = message.content,
            autoDelete = message.autoDelete,
            createdAt = DateTime.UtcNow,
            owner = userId
        };
        await _textStoreService.UploadText(messageContent);
        
        string downloadUrl = ConfigEnum.LocalHost + "/file/download/" + CustomEncoder.EncodeNumber(messageContent.id);
        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Url to view your messgae",
            content = downloadUrl
        });
    }

    [HttpGet("download/{messageId}")]
    public async Task<IActionResult> ViewMessage([FromRoute] string messageId)
    {
        // Decode Base64 encodedId to get the true text Id
        var textIde = CustomEncoder.DecodeNumber(messageId);

        var text = await _textStoreService.GetById(textIde);
        if (text is null)
            return NotFound("Not found this massage");
        

        // if user want to delete after dowload
        if (text.autoDelete)
        {
            _textStoreService.DeleteFile(text.id);
        }

        // Return the file as a downloadable content
        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Message to you",
            content = text.content
        });

    }

   


}