using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entity;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly IService<SongDto> _service;
        public SongController(IService<SongDto> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SongDto>>> GetAsync(int page = 1, int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            try
            {
                var songs = await _service.GetAllAsync();
                var pagedSongs = songs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(pagedSongs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Song/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SongDto>> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var song = await _service.GetAsync(id);
                if (song == null)
                    return NotFound("Song not found.");
                return Ok(song);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Song
        [HttpPost]
        [Authorize(Roles = "singer")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<SongDto>> PostAsync([FromForm] SongDto value)
        {
            if (value == null)
                return BadRequest("Song data is required.");
            if (value.File == null)
                return BadRequest("A video file is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.SingerId.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                string directory = Path.Combine(Environment.CurrentDirectory, "Video");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var filePath = Path.Combine(Environment.CurrentDirectory, "Video", value.File.FileName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }

                var createdSong = await _service.AddAsync(value);
                return Ok(createdSong);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Song/5
        [HttpPut("{id}")]
        [Authorize(Roles = "singer")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<SongDto>> PutAsync(int id, [FromForm] SongDto value)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");
            if (value == null)
                return BadRequest("Song data is required.");
            if (value.File == null)
                return BadRequest("A video file is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.SingerId.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                string directory = Path.Combine(Environment.CurrentDirectory, "Video");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var filePath = Path.Combine(Environment.CurrentDirectory, "Video", value.File.FileName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }

                value.Type = ((Types)Enum.Parse(typeof(Types), value.Type)).ToString();
                value.Category = ((Categories)Enum.Parse(typeof(Categories), value.Category)).ToString();

                var updatedSong = await _service.UpdateAsync(id, value);
                return Ok(updatedSong);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Song/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "singer")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("audio/{id}")]
        public async Task<IActionResult> GetSongAudio(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var song = await _service.GetAsync(id);
                if (song == null || song.Video == null)
                    return NotFound("Audio not found");

                return File(song.Video, "video/mp4", $"{song.Name}.mp4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
