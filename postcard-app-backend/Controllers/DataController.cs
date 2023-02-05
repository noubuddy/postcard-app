using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace postcard_app_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly CascadeClassifier faceClassifier;

    public DataController()
    {
        // Cv2.UseOptimized();
        string path = Environment.CurrentDirectory + "/haarcascade_frontalface_default.xml";
        faceClassifier = new CascadeClassifier(path);
    }

    private IFormFile CropFace(IFormFile image)
    {
        using var memoryStream = new MemoryStream();
        image.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        using var src = Cv2.ImDecode(memoryStream.ToArray(), ImreadModes.Color);
        var faces = faceClassifier.DetectMultiScale(src);
        
        if (faces.Length <= 0)
        {
            Console.WriteLine("No faces detected in the image.");
            return null;
        }
        
        Console.WriteLine("Face detected.");

        var face = faces.First();
        var cropped = new Mat(src, face);
        var croppedBytes = cropped.ToBytes();
        
        var stream = new MemoryStream(croppedBytes);
        IFormFile croppedImage = new FormFile(stream, 0, croppedBytes.Length, "name", "fileName.jpeg");

        return croppedImage;
    }

    // Get image by filename
    [HttpGet("images/{filename}")]
    public IActionResult GetImage(string filename)
    {
        try
        {
            string[] type = filename.Split(".");
            var image = System.IO.File.OpenRead(Environment.CurrentDirectory + "/Upload/" + filename);
            return File(image, "image/" + type[type.Length - 1]);
        }
        catch
        {
            return BadRequest("Failed to get image");
        }
    }
    
    [HttpPost("images")]
    public IActionResult Upload([FromForm] IFormFile image)
    {
        // path to save uploaded image
        string path = Environment.CurrentDirectory + "/Upload/";
    
        // return bad request if image size is null
        if (image.Length <= 0) return BadRequest("Image uploading failed");
    
        image = CropFace(image);

        if (image.Length <= 0)
            return BadRequest("No faces detected");

        // create /Upload/ directory if it's exists
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    
        // create stream to copy file to
        using FileStream fs = System.IO.File.Create(path + image.FileName);
        // copy file to the stream
        image.CopyTo(fs);
        // clear buffers for the stream
        fs.Flush();
        // return cropped image
        return Ok("http://localhost:3000/api/Data/images/" + image.FileName);
    }
}