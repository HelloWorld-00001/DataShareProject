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
using WebApplication1.Service.IS3Service;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/message")]
public class TextStoreController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly string _folderStored = "ShareMessage/";
    private readonly IS3Service _s3Service;
    // init interface for this class
    readonly ITextStoreService _textStoreService;

    public TextStoreController(ITextStoreService textStoreService, IHttpContextAccessor httpContext, IS3Service s3Servic)
    {
        _httpContext = httpContext;
        _textStoreService = textStoreService;
        _s3Service = s3Servic;
    }
    
    [HttpPost, Authorize]
    public async Task<IActionResult> UploadText([FromForm] TextStoreDtos message)
    {
        
        int userId = JwtHeper.GetUserId(this);

        if (message.content.Length < 1 || message.content.IsNullOrEmpty())
            return BadRequest("No content in your massage");

        var messageContent = new TextStore
        {
            folderStored = _folderStored,
            bucketName = ConfigEnum.S3BucketName,
            autoDelete = message.autoDelete,
            createdAt = DateTime.UtcNow,
            owner = userId
        };
        // save meta data to db
        await _textStoreService.UploadText(messageContent);
        // upload message to cloud (s3)
        await _s3Service.UploadTextAsync(_folderStored + messageContent.id.ToString(), message.content);
        
        string downloadUrl = ConfigEnum.LocalHost + "/file/view/" + CustomEncoder.EncodeNumber(messageContent.id);
        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Url to view your messgae",
            content = downloadUrl
        });
    }

    [HttpGet("view/{messageKey}")]
    public async Task<IActionResult> ViewMessage([FromRoute] string messageKey)
    {
        if (messageKey.IsNullOrEmpty())
            return BadRequest("Please enter key");
        // Decode Base64 encodedId to get the true text Id
        var textIde = CustomEncoder.DecodeNumber(messageKey);

        var text = await _textStoreService.GetById(textIde);
        if (text is null)
            return NotFound("Not found this massage");
        
        // get content from s3
        var stream = await _s3Service.DownloadFileAsync(_folderStored + text.id.ToString());
        string result;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = await reader.ReadToEndAsync();
        }

        // if user want to delete after dowload
        if (text.autoDelete)
        {
            _textStoreService.DeleteFile(text.id);
            _s3Service.DeleteFileAsync(_folderStored + text.id.ToString());
        }

        // Return the file as a downloadable content
        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Message to you",
            content = result
        });

    }

   


}