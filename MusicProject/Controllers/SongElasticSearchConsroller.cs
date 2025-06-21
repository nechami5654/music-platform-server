using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongElasticSearchController : ControllerBase
    {
        private readonly ISongElasticSearchService _songService;

        public SongElasticSearchController(ISongElasticSearchService songService)
        {
            _songService = songService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchSongsAsync([FromQuery] string lyrics, [FromQuery] int page = 1, [FromQuery] int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            if (string.IsNullOrWhiteSpace(lyrics))
                return BadRequest("Lyrics query cannot be empty.");

            try
            {
                var allResults = await _songService.SearchSongsByLyricsAsync(lyrics);
                var paged = allResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("free-search")]
        public async Task<IActionResult> FreeSearchAsync([FromQuery] string text, [FromQuery] int page = 1, [FromQuery] int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Search text cannot be empty.");

            try
            {
                var results = await _songService.FreeSearchAsync(text);
                var paged = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
