using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _ImgBBApiKey = "3ae7fab6d966ddf567d1ca534701d383";

        [HttpPost("UploadImage")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No se ha seleccionado ninguna imagen...");
            }

            Console.WriteLine($"Archivo recibido: {image.FileName}, Tamaño: {image.Length} bytes");

            using var MemoryStream = new MemoryStream();
            await image.CopyToAsync(MemoryStream);
            var ImageBytes = MemoryStream.ToArray();
            var ByteArrayContent = new ByteArrayContent(ImageBytes);
            ByteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);

            using var Client = new HttpClient();
            var Content = new MultipartFormDataContent
            {
                { ByteArrayContent, "image", image.FileName}
            };


            var Response = await Client.PostAsync($"https://api.imgbb.com/1/upload?key={_ImgBBApiKey}", Content);
            if (!Response.IsSuccessStatusCode)
            {
                var errorResponse = await Response.Content.ReadAsStringAsync();
                return BadRequest("Error al subir la imagen...");
            }

            var JsonResponse = await Response.Content.ReadAsStringAsync();
            var imageurl = JObject.Parse(JsonResponse)["data"]?["url"]?.ToString();
            return Ok(new {imageurl});
        }
    }
}
