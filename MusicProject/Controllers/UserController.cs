using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UserController : ControllerBase
    {
        private readonly IService<UserDto> _service;
        public static string DirectoryPath = Path.Combine(Environment.CurrentDirectory, "Images");

        public UserController(IService<UserDto> service)
        {
            _service = service;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAsync()
        {
            try
            {
                var users = await _service.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var user = await _service.GetAsync(id);
                if (user == null)
                    return NotFound("User not found.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostAsync([FromForm] UserDto value)
        {
            if (value == null)
                return BadRequest("User data is required.");
            if (value.File == null)
                return BadRequest("User image file is required.");

            try
            { 
                var filePath = Path.Combine(Environment.CurrentDirectory, "Images", value.File.FileName);
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }

                var createdUser = await _service.AddAsync(value);
                return Ok(createdUser);            
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [Authorize(Roles = "user")]

        public async Task<ActionResult<UserDto>> PutAsync(int id, [FromForm] UserDto value)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");
            if (value == null)
                return BadRequest("User data is required.");
            if (value.File == null)
                return BadRequest("User image file is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != id.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }

            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, "Images", value.File.FileName);
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await value.File.CopyToAsync(fs);
                }

                var updatedUser = await _service.UpdateAsync(id, value);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteAsync(int id)
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
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /getImageUser/5
        [HttpGet("getImageUser/{id}")]
        public async Task<IActionResult> GetImageAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var user = await _service.GetAsync(id);
                if (user == null || user.Image == null)
                    return NotFound("Image not found.");
                return File(user.Image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
