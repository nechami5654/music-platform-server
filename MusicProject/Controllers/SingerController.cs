using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entity;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingerController : ControllerBase
    {
        private readonly IService<SingerDto> _service;
        public static string directory = Environment.CurrentDirectory + "/Images/";

        public SingerController(IService<SingerDto> service)
        {
            _service = service;
        }

        // GET: api/Singer
        [HttpGet]
        public async Task<ActionResult<List<SingerDto>>> GetAsync()
        {
            try
            {
                var singers = await _service.GetAllAsync();
                return Ok(singers);
            }
            catch(Exception ex)

            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Singer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SingerDto>> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var singer = await _service.GetAsync(id);
                return Ok(singer);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Singer
        [HttpPost]
        public async Task<ActionResult<SingerDto>> PostAsync([FromForm] SingerDto value)
        {
            if(value == null)
                return BadRequest("Singer data is required.");
            if(value.File == null)
                return BadRequest("Image file is required.");

            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, "Image", value.File.FileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }
                
                var singer = await _service.AddAsync(value);
                return Ok(singer);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Singer/5
        [HttpPut("{id}")]
        [Authorize(Roles = "singer")]

        public async Task<ActionResult<SingerDto>> PutAsync(int id, [FromForm] SingerDto value)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");
            if(value == null)
                return BadRequest("Singer data is required.");
            if(value.File == null)
                return BadRequest("Image file is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, "Image", value.File.FileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }

                var singer = await _service.UpdateAsync(id, value);
                return Ok(singer);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Singer/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "singer")]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            if(id <= 0)
                return BadRequest("Invalid id.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /getImageSinger/5
        [HttpGet("getImageSinger/{id}")]
        public async Task<IActionResult> GetImageAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var singer = await _service.GetAsync(id);
                if (singer == null || singer.Image == null)
                    return NotFound("Image not found.");
                return File(singer.Image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
