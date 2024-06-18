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

namespace WebApplication1.Controller;

[ApiController]
[Route("api/file")]
public class FileStoreController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContext;

    // init interface for this class
    readonly IFileStoreService _fileStoreService;

    public FileStoreController(IFileStoreService fileStoreService, IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
        _fileStoreService = fileStoreService;
    }
    
    [HttpPost, Authorize]
    public async Task<IActionResult> UploadFile([FromForm] FileStoreDtos fileData)
    {
        if (fileData.fileControl == null || fileData.fileControl.Length < 1)
            return BadRequest("No file uploaded.");
        
        // get user id from jwt
        int userId = JwtHeper.GetUserId(this);

        using (var memoryStream = new MemoryStream())
        {
            await fileData.fileControl.CopyToAsync(memoryStream);

            var fileStore = new FileStore
            {
                fileName = fileData.fileControl.FileName,
                fileData = memoryStream.ToArray(),
                uploadedAt = DateTime.UtcNow,
                fileSize = (int)fileData.fileControl.Length,
                autoDelete = fileData.autoDelete,
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
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> DownloadFile([FromRoute] string fileId)
    {
        // Decode Base64 encodedId to get the true fileStore.Id

        var fileIde = CustomEncoder.DecodeNumber(fileId);

        var fileStore = await _fileStoreService.GetById(fileIde);
        if (fileStore is null)
            return NotFound("Not found this file");
        

        // if user want to delete after dowload
        if (fileStore.autoDelete)
        {
            _fileStoreService.DeleteFile(fileStore.id);
        }
        
        // Return the file as downloadable content
        var fileContentResult = new FileContentResult(fileStore.fileData, "application/octet-stream")
        {
            FileDownloadName = fileStore.fileName
        };
        // Return the file as a downloadable content
        return File(fileStore.fileData, "application/octet-stream", fileStore.fileName);

    }

   


}