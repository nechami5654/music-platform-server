using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Repository.Entity;
using Service.Interface;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionHistoryController : ControllerBase
    {
        private readonly IExtensionHistoryService _service;
        public ExtensionHistoryController(IExtensionHistoryService service)
        {
            _service = service;
        }
        [HttpGet("{idUser}/{idSong}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<bool>> GetAsync(int idUser, int idSong)
        {
            if (idUser <= 0 || idSong <= 0)
                return BadRequest("Invalid id.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != idUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                var history = await _service.GetAsync(idUser, idSong);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{idUser}")]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<List<SongDto>>> GetAllAsync(int idUser, int page = 1, int pageSize = 6)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid paging parameters.");

            if (idUser <= 0)
                return BadRequest("Invalid id.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != idUser.ToString())
            {
                return Unauthorized("You are not authorized to get this song.");
            }

            try
            {
                var history = await _service.GetAllAsync(idUser);
                var paged = history.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
