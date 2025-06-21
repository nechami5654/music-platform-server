using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
    public class ExtensionSongController : ControllerBase
    {
        private readonly IService<SongDto> _service;
        private readonly ISongService _songService;
        public static string directory = Path.Combine(Environment.CurrentDirectory, "Video");

        public ExtensionSongController(ISongService songService, IService<SongDto> service)
        {
            _songService = songService;
            _service = service;
        }

        [HttpGet("getVideo/{id}")]
        public async Task<IActionResult> GetVideoAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var songDto = await _service.GetAsync(id);
                if (songDto == null || songDto.Video == null)
                    return NotFound("Video not found.");

                return File(songDto.Video, "video/mp4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("filterSong")]
        public async Task<ActionResult<List<SongDto>>> FilterSongAsync(string? name, bool? newSongs, bool? popularSongs, string? nameSinger, List<string>? similarWords, Types? type, Categories? category, TargetAge? age, int page = 1, int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            try
            {
                var songs = await _songService.FilterSongAsync(name, newSongs, popularSongs, nameSinger, type, category, age);
                var paged = songs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addView")]
        public async Task<IActionResult> AddViewAsync(int idSong)
        {
            if (idSong <= 0)
                return BadRequest("Invalid song id.");

            try
            {
                await _songService.AddViewAsync(idSong);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addLike")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddLikeAsync(int idSong, int idUser)
        {
            if (idSong <= 0 || idUser <= 0)
                return BadRequest("Invalid song id or user id.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != idUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                await _songService.AddLikeAsync(idSong, idUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("removeLike")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> RemoveLikeAsync(int idSong, int idUser)
        {
            if (idSong <= 0 || idUser <= 0)
                return BadRequest("Invalid song id or user id.");
            var currentUserId = User.FindFirst("id")?.Value;

            if (currentUserId != idUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                await _songService.RemoveLikeAsync(idSong, idUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("likeByUser")]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<bool>> LikeByUserAsync(int idUser, int idSong)
        {
            if (idSong <= 0 || idUser <= 0)
                return BadRequest("Invalid song id or user id.");
            var currentUserId = User.FindFirst("id")?.Value;

            if (currentUserId != idUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                var liked = await _songService.LikeByUserAsync(idUser, idSong);
                return Ok(liked);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("search-by-audio")]
        public async Task<IActionResult> SearchSongsByAudioAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var songs = await _songService.SearchSongsByAudioAsync(filePath);
                System.IO.File.Delete(filePath);
                return Ok(songs);
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getSongsBySinger/{singerId}")]
        public async Task<ActionResult<List<SongDto>>> GetSongsBySingerIdAsync(int singerId, int page = 1, int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            if (singerId <= 0)
                return BadRequest("Invalid singer id.");
            
            try
            {
                var songs = await _songService.GetSongsBySingerIdAsync(singerId);
                var paged = songs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
