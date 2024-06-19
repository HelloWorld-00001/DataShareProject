using DataShareCore.Common.Enum;
using DataShareCore.Dtos;
using DataShareCore.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Service.AccountService;
using WebApplication1.Service.FileStoreService;
using System.Data;
using System;
using System.Security.Claims;
using System.Web;
using DataShareCore.Common.Helper;
using DataShareCore.Response;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Service.IS3Service;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/file")]
public class FileStoreController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly IS3Service _s3Service;
    private readonly string _folderStored = "ShareFile/";

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
            await _s3Service.UploadFileAsync(_folderStored + file.fileControl.FileName, stream);
        }
        
        var fileStore = new FileStore
        {
            fileName = file.fileControl.FileName,
            folderStored = _folderStored,
            bucketName = ConfigEnum.S3BucketName,
            uploadedAt = DateTime.UtcNow,
            fileSize = (int)file.fileControl.Length,
            autoDelete = file.autoDelete,
            owner = userId
        };

        await _fileStoreService.UploadFile(fileStore);

        string downloadUrl = ConfigEnum.LocalHost + "/file/download/" + CustomEncoder.EncodeNumber(fileStore.id);

        return Ok(new APIResponser<string>
        {
            success = true,
            message = "Url to download your file",
            content = downloadUrl
        });
        
    }

    [HttpGet("download/{fileKey}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string fileKey)
    {
        // Decode Base64 encodedId to get the true fileStore.Id

        var fileIde = CustomEncoder.DecodeNumber(fileKey);

        var fileStore = await _fileStoreService.GetById(fileIde);
        if (fileStore is null)
            return NotFound("Not found this file");
        

        // get the file
        var stream = await _s3Service.DownloadFileAsync(_folderStored + fileStore.fileName);

        // if user want to delete after dowload
        if (fileStore.autoDelete)
        {
            _fileStoreService.DeleteFile(fileStore.id);
            await _s3Service.DeleteFileAsync(_folderStored + fileStore.fileName);
        }
        
        // Return the file as downloadable content
        return File(stream, "application/octet-stream", fileStore.fileName);

    }

   


}