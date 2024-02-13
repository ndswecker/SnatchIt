using Microsoft.AspNetCore.Mvc;

namespace SnatchItAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        [HttpGet("GetFiles")]
        public async Task<IActionResult> ListAllBlobs()
        {
            return Ok();
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok();
        }

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> Download(string filename)
        {
            return Ok();
        }
    }
}