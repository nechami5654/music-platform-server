using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechToTextController : ControllerBase
    {
        private readonly ISpeechToTextService _speechToTextService;

        public SpeechToTextController(ISpeechToTextService speechToTextService)
        {
            _speechToTextService = speechToTextService;
        }

        [HttpPost("search-by-audio")]
        public async Task<IActionResult> SearchByAudioAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var lyrics = await _speechToTextService.RecognizeSpeechAsync(filePath);
                if (string.IsNullOrEmpty(lyrics))
                    return BadRequest("No speech recognized.");

                return Ok(lyrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the audio file: {ex.Message}");
            }
        }
    }
}
