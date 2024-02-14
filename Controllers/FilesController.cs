using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;

namespace SnatchItAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly AzureBlobService _blobService;
        private readonly IConfiguration _config;
        public FilesController(IConfiguration config)
        {
            _blobService = new AzureBlobService(config);
            _config = config;
        }

        [HttpGet("ListAllBlobs")]
        public async Task<IActionResult> ListAllBlobs()
        {
            var containerDtos = await _blobService.ListBlobContainersAsync();
            return Ok(containerDtos);
        }


        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok();
        }

        [HttpGet("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            return Ok();
        }

        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            return Ok();
        }
    }
}