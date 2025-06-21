using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IService<FeedbackDto> _service;

        public FeedbackController(IService<FeedbackDto> service)
        {
            _service = service;
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<ActionResult<List<FeedbackDto>>> GetAsync()
        {
            try
            {
                var feedbacks = await _service.GetAllAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Feedback/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackDto>> GetAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            try
            {
                var feedback = await _service.GetAsync(id);
                if (feedback == null)
                    return NotFound("Feedback not found.");
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Feedback
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<FeedbackDto>> PostAsync([FromForm] FeedbackDto value)
        {
            if (value == null)
                return BadRequest("Feedback data is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.IdUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                var createdFeedback = await _service.AddAsync(value);
                return Ok(createdFeedback);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // PUT: api/Feedback/5
        [HttpPut("{id}")]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<FeedbackDto>> PutAsync(int id, [FromForm] FeedbackDto value)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");
            if (value == null)
                return BadRequest("Feedback data is required.");

            var currentUserId = User.FindFirst("id")?.Value;
            if (currentUserId != value.IdUser.ToString())
            {
                return Unauthorized("You are not authorized to upload this song.");
            }
            try
            {
                var updatedFeedback = await _service.UpdateAsync(id, value);
                return Ok(updatedFeedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Feedback/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

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
    }
}
