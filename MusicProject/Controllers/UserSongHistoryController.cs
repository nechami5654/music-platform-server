using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Repository.Entity;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSongHistoryController : ControllerBase
    {
        private readonly IService<UserSongHistoryDto> _service;

        public UserSongHistoryController(IService<UserSongHistoryDto> service)
        {
            _service = service;
        }

        // GET: api/UserSongHistory
        [HttpGet]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<List<UserSongHistoryDto>>> GetAsync()
        {

            try
            {
                var histories = await _service.GetAllAsync();
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/UserSongHistory/5
        [HttpGet("{id}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<UserSongHistoryDto>> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                var history = await _service.GetAsync(id);
                if (history == null)
                    return NotFound("User song history not found.");
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/UserSongHistory
        [HttpPost]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<UserSongHistoryDto>> PostAsync([FromQuery] UserSongHistoryDto value)
        {
            if (value == null)
                return BadRequest("User song history data is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.UserId.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                var createdHistory = await _service.AddAsync(value);
                return Ok(createdHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/UserSongHistory/5
        [HttpPut("{id}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<UserSongHistoryDto>> PutAsync(int id, [FromForm] UserSongHistoryDto value)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");
            if (value == null)
                return BadRequest("User song history data is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.UserId.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                var updatedHistory = await _service.UpdateAsync(id, value);
                return Ok(updatedHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/UserSongHistory/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                await _service.DeleteAsync(id);
                return  Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
