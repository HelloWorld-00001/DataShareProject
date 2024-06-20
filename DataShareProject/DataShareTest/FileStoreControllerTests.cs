using Moq;
using NUnit.Framework;
using WebApplication1.Controller;
using WebApplication1.Service.FileStoreService;
using WebApplication1.Service.IS3Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataShareCore.Dtos;
using DataShareCore.Models;
using DataShareCore.Response;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataShareCore.Common.Helper;

namespace DataShareTest
{
    public class FileStoreControllerTests
    {
        private Mock<IFileStoreService> _fileStoreServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IS3Service> _s3ServiceMock;
        private FileStoreController _controller;

        [SetUp]
        public void SetUp()
        {
            _fileStoreServiceMock = new Mock<IFileStoreService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _s3ServiceMock = new Mock<IS3Service>();
            
            // Create a mock HttpContext with the necessary claims
            var claims = new[]
            {
                new Claim("Id", "2"),
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "2")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = claimsPrincipal };

            // Mock IHttpContextAccessor to return the mock HttpContext
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new FileStoreController(_fileStoreServiceMock.Object, _httpContextAccessorMock.Object, _s3ServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        
        
        // ------------------------------------------------------------------------------------------
        // Upload file test area
        // ------------------------------------------------------------------------------------------

        // Upload file test cases
        [Test]
        public async Task UploadFile_NoFileUploaded_ReturnsBadRequest()
        {
            // Arrange
            var file = new FileStoreDtos
            {
                fileControl = null
            };

            // Act
            var result = await _controller.UploadFile(file);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("No file uploaded.", badRequestResult.Value);
        }

        [Test]
        public async Task UploadFile_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var file = new FileStoreDtos
            {
                fileControl = fileMock.Object,
                autoDelete = false
            };

            // Mock the S3 service upload
            _s3ServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

            // Mock the file store service upload
            _fileStoreServiceMock.Setup(s => s.UploadFile(It.IsAny<FileStore>())).ReturnsAsync(new FileStore { id = 1 });

            // Act
            var result = await _controller.UploadFile(file);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as APIResponser<string>;
            Assert.IsTrue(response.success);
            Assert.AreEqual("Url to download your file", response.message);
        }
        
        [Test]
        public async Task UploadFile_PDF_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var file = new FileStoreDtos
            {
                fileControl = fileMock.Object,
                autoDelete = false
            };

            // Mock the S3 service upload
            _s3ServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

            // Mock the file store service upload
            _fileStoreServiceMock.Setup(s => s.UploadFile(It.IsAny<FileStore>())).ReturnsAsync(new FileStore { id = 1 });

            // Act
            var result = await _controller.UploadFile(file);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as APIResponser<string>;
            Assert.IsTrue(response.success);
            Assert.AreEqual("Url to download your file", response.message);
        }
        
        [Test]
        public async Task UploadFile_MP4_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test.mp4");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var file = new FileStoreDtos
            {
                fileControl = fileMock.Object,
                autoDelete = false
            };

            // Mock the S3 service upload
            _s3ServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

            // Mock the file store service upload
            _fileStoreServiceMock.Setup(s => s.UploadFile(It.IsAny<FileStore>())).ReturnsAsync(new FileStore { id = 1 });

            // Act
            var result = await _controller.UploadFile(file);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as APIResponser<string>;
            Assert.IsTrue(response.success);
            Assert.AreEqual("Url to download your file", response.message);
        }
        
        [Test]
        public async Task UploadFile_NoPostType_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("test");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var file = new FileStoreDtos
            {
                fileControl = fileMock.Object,
                autoDelete = false
            };

            // Mock the S3 service upload
            _s3ServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

            // Mock the file store service upload
            _fileStoreServiceMock.Setup(s => s.UploadFile(It.IsAny<FileStore>())).ReturnsAsync(new FileStore { id = 1 });

            // Act
            var result = await _controller.UploadFile(file);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as APIResponser<string>;
            Assert.IsTrue(response.success);
            Assert.AreEqual("Url to download your file", response.message);
        }
        
        // ------------------------------------------------------------------------------------------
        // End Upload file test area
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // Upload message test area
        // ------------------------------------------------------------------------------------------
        
        // ------------------------------------------------------------------------------------------
        // End message  test area
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // Download/view file test area
        // ------------------------------------------------------------------------------------------
        [Test]
        public async Task DownloadFile_ValidFileKey_ReturnsFile()
        {
            // Arrange
            var fileId = 1;
            var fileStore = new FileStore
            {
                id = fileId,
                fileName = "test.txt",
                folderStored = "ShareFile/",
                type = "File",
                autoDelete = false
            };

            _fileStoreServiceMock.Setup(s => s.GetById(fileId)).ReturnsAsync(fileStore);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
            _s3ServiceMock.Setup(s => s.IsExists(It.IsAny<string>())).ReturnsAsync(true);
            _s3ServiceMock.Setup(s => s.DownloadFileAsync(It.IsAny<string>())).ReturnsAsync(stream);

            // Act
            var result = await _controller.DownloadFile(CustomEncoder.EncodeNumber(fileId));

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.AreEqual("test.txt", fileResult.FileDownloadName);
            Assert.AreEqual("application/octet-stream", fileResult.ContentType);
        }

        [Test]
        public async Task DownloadFile_InvalidFileKey_ReturnsInvalid()
        {
            // Arrange
            var fileId = 999; // Assuming a non-existent fileId
            _fileStoreServiceMock.Setup(s => s.GetById(fileId)).ReturnsAsync((FileStore)null);

            // Act
            var result = await _controller.DownloadFile(fileId.ToString());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var notFoundResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid key", notFoundResult.Value);
        }

        [Test]
        public async Task DownloadFile_FileNotExistInS3_ReturnsNotFound()
        {
            // Arrange
            var fileId = 1;
            var fileStore = new FileStore
            {
                id = fileId,
                fileName = "test.txt",
                folderStored = "ShareFile/",
                type = "File",
                autoDelete = false
            };

            _fileStoreServiceMock.Setup(s => s.GetById(fileId)).ReturnsAsync(fileStore);
            _s3ServiceMock.Setup(s => s.IsExists(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _controller.DownloadFile(CustomEncoder.EncodeNumber(1));

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Can not found this file or message", notFoundResult.Value);
        }

        // Add more test cases as per your requirements (autoDelete, message type handling, etc.)
        [Test]
        public async Task DownloadFile_TextFileType_ReturnsTextContent()
        {
            // Arrange
            var fileId = 1;
            var fileStore = new FileStore
            {
                id = fileId,
                fileName = "message.txt", // Assuming a text file
                folderStored = "ShareMessage/",
                type = "Text",
                autoDelete = false
            };

            _fileStoreServiceMock.Setup(s => s.GetById(fileId)).ReturnsAsync(fileStore);

            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, this is a test message!"));
            _s3ServiceMock.Setup(s => s.IsExists(It.IsAny<string>())).ReturnsAsync(true);
            _s3ServiceMock.Setup(s => s.DownloadFileAsync(It.IsAny<string>())).ReturnsAsync(mockStream);

            // Act
            var result = await _controller.DownloadFile(CustomEncoder.EncodeNumber(1));

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<APIResponser<string>>(okResult.Value);

            var response = okResult.Value as APIResponser<string>;
            Assert.IsTrue(response.success);
            Assert.AreEqual("Message to you", response.message);
            Assert.AreEqual("Hello, this is a test message!", response.content);
        }
        
        
        [Test]
        public async Task DownloadFile_AutoDeleteTrue_DeletesFileAfterDownload()
        {
            // Arrange
            var fileId = 1;
            var fileName = "test.txt";
            var fileStore = new FileStore
            {
                id = fileId,
                fileName = fileName,
                folderStored = "ShareFile/",
                type = "File",
                autoDelete = true // Setting autoDelete to true
            };

            _fileStoreServiceMock.Setup(s => s.GetById(fileId)).ReturnsAsync(fileStore);

            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes("File content"));
            _s3ServiceMock.Setup(s => s.IsExists(It.IsAny<string>())).ReturnsAsync(true);
            _s3ServiceMock.Setup(s => s.DownloadFileAsync(It.IsAny<string>())).ReturnsAsync(mockStream);

            // Mock DeleteFileAsync method of IS3Service
            _s3ServiceMock.Setup(s => s.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DownloadFile(CustomEncoder.EncodeNumber(1));

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.AreEqual(fileName, fileResult.FileDownloadName);

            // Check that DeleteFileAsync was called
            _s3ServiceMock.Verify(s => s.DeleteFileAsync(It.IsAny<string>()), Times.Once);
        }




    }
}
