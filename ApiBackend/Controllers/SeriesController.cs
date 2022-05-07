using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
using ApiBackend.Models;

namespace ApiBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriesController : ControllerBase
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        public SeriesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
       
        [HttpPost, DisableRequestSizeLimit]
        public ObjectResult ImportFile()
        {
            try
            {
               
                string folderName, webRootPath, newPath, response;

                var file = Request.Form.Files[0];

                if (file.Length <= 0)
                {
                    return BadRequest("Empty File!");
                }

                folderName = "Upload";
                webRootPath = _hostingEnvironment.WebRootPath;
                
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    newPath = Path.Combine(Directory.GetCurrentDirectory(), folderName); 
                } else
                {
                    newPath = Path.Combine(webRootPath, folderName);
                }
                
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                InputFile seriesFile = new(file,newPath);
                response = seriesFile.getResponse();
                
                return Ok(response);
            
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }
}