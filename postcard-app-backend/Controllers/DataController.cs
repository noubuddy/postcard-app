using Microsoft.AspNetCore.Mvc;

namespace postcard_app_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    [HttpPost("{image}")]
    public IActionResult Upload([FromForm] Models.File image)
    {
        string path = Environment.CurrentDirectory + "/Upload/";

        // return bad request if image size is null
        if (image.Image.Length <= 0) return BadRequest("Image uploading failed");

        // create /Upload/ directory if it's exists
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        // create stream to copy file to
        using FileStream fs = System.IO.File.Create(path + image.Image.FileName);
        // copy file to the stream
        image.Image.CopyTo(fs);
        // clear buffers for the stream
        fs.Flush();
        // return path to received file
        return Ok(path + image.Image.FileName);
    }
    
    // GET api/values
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "value1", "value2" };
    }
}