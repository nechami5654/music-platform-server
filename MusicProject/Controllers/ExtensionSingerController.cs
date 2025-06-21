using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionSingerController : ControllerBase
    {
        private readonly ISingerService _service;

        public ExtensionSingerController(ISingerService service)
        {
            _service = service;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> PostAsync([FromQuery] string singerName, [FromQuery] string pass)
        {
            if (string.IsNullOrWhiteSpace(singerName) || string.IsNullOrWhiteSpace(pass))
                return BadRequest("Singer name and password are required.");

            try
            {
                var singer = await _service.AuthenticateAsync(singerName, pass);
                if (singer != null)
                {
                    var token = _service.Generate(singer);
                    return Ok(token);
                }
                return Unauthorized("Singer does not exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("validSingerPassword")]
        [Authorize(Roles = "singer")]
        public async Task<IActionResult> ValidSingerPasswordAsync([FromQuery] int id, [FromQuery] string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid singer ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                bool response = await _service.ValidSingerPasswordAsync(id, password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
