using DataShareCore.Common.Enum;
using DataShareCore.Dtos;
using DataShareCore.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Service.AccountService;
using WebApplication1.Service.FileStoreService;
using System.Data;
using System;
using System.Security.Claims;
using System.Text;
using System.Web;
using DataShareCore.Common.Helper;
using DataShareCore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Service.IS3Service;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/file")]
public class FileStoreController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly IS3Service _s3Service;
    private readonly string _fileStored = "ShareFile/";
    private readonly string _textStored = "ShareMessage/";
    private  readonly string _textType = "Text";
    private  readonly string _fileType = "File";
    // init interface for this class
    readonly IFileStoreService _fileStoreService;

    public FileStoreController(IFileStoreService fileStoreService, IHttpContextAccessor httpContext, IS3Service s3Servic)
    {
        _httpContext = httpContext;
        _fileStoreService = fileStoreService;
        _s3Service = s3Servic;
    }
    
    [HttpPost, Authorize]
    public async Task<IActionResult> UploadFile([FromForm] FileStoreDtos file)
    {
        if (file.fileControl == null || file.fileControl.Length < 1)
            return BadRequest("No file uploaded.");
        
        // get user id from jwt
        int userId = JwtHeper.GetUserId(this);
        
        using (var stream = file.fileControl.OpenReadStream())
        {
            await _s3Service.UploadFileAsync(_fileStored + file.fileControl.FileName, stream);
        }
        
        var fileStore = new FileStore
        {
            fileName = file.fileControl.FileName,
            folderStored = _fileStored,
            bucketName = ConfigEnum.S3BucketName,
            uploadedAt = DateTime.UtcNow,
            fileSize = (int)file.fileControl.Length,
            autoDelete = file.autoDelete,
            type = _fileType,
            owner = userId
        };

        await _fileStoreService.UploadFile(fileStore);

        string downloadUrl = ConfigEnum.LocalHost + "file/download/" + CustomEncoder.EncodeNumber(fileStore.id);

        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Url to download your file",
            content = downloadUrl
        });
        
    }
    
    [HttpPost("message"), Authorize]
    public async Task<IActionResult> UploadText([FromForm] TextStoreDtos message)
    {
        
        int userId = JwtHeper.GetUserId(this);

        if (message.content.Length < 1 || message.content.IsNullOrEmpty())
            return BadRequest("No content in your massage");

        var fileStore = new FileStore
        {
            fileName = "",
            folderStored = _textStored,
            bucketName = ConfigEnum.S3BucketName,
            uploadedAt = DateTime.UtcNow,
            fileSize = message.content.Length,
            autoDelete = message.autoDelete,
            type = _textType,
            owner = userId
        };

        // save meta data to db
        await _fileStoreService.UploadFile(fileStore);
        // upload message to cloud (s3)
        await _s3Service.UploadTextAsync(_textStored + fileStore.id.ToString(), message.content);
        
        string downloadUrl = ConfigEnum.LocalHost + "file/download/" + CustomEncoder.EncodeNumber(fileStore.id);
        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Url to view your messgae",
            content = downloadUrl
        });
    }

    [HttpGet("download/{fileKey}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string fileKey)
    {
        // Decode Base64 encodedId to get the true fileStore.Id
        if (fileKey.Length < 1 || fileKey.Length > 63)
            return BadRequest("Key is not valid");

        var fileId = CustomEncoder.DecodeNumber(fileKey);

        var fileStore = await _fileStoreService.GetById(fileId);
        
        if (fileStore is null)
            return NotFound("Not found this file");

        // if the request is file type
        
        // get the file
        var fileNameOnAWS = fileStore.folderStored;
        fileNameOnAWS += (fileStore.type.Equals(_textType) ? fileStore.id.ToString() : fileStore.fileName);

        // check if the file exist
        bool isExist = await _s3Service.IsExists(fileNameOnAWS);
        if (!isExist)
            return NotFound("Can not found this file or message");
        
        var stream = await _s3Service.DownloadFileAsync(fileNameOnAWS);

        // if user want to delete after dowload
        if (fileStore.autoDelete)
        {
            _fileStoreService.DeleteFile(fileStore.id);
            await _s3Service.DeleteFileAsync(fileNameOnAWS);
        }

        //if the request is message type --> return a string
        if (fileStore.type.Equals(_textType))
        {
            string result = await FileToString(stream);
            // Return the file as a downloadable content
            return Ok(new APIResponser<string>
            {
                success = true,
                message = "Message to you",
                content = result
            });
        }
        
        // Return the file as downloadable content
        return File(stream, "application/octet-stream", fileStore.fileName);
        

    }


    private async Task<string> FileToString(Stream stream)
    {
        string result;

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = await reader.ReadToEndAsync();
        }

        return result;
    }

   


}