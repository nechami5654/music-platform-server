using System;
using System.Threading.Tasks;
using Repository.Interface;
using Service.Interface;

namespace Service.Service
{
    public class SpeechToTextService : ISpeechToTextService
    {
        private readonly ISpeechToTextRepository _speechToTextRepository;

        public SpeechToTextService(ISpeechToTextRepository speechToTextRepository)
        {
            _speechToTextRepository = speechToTextRepository;
        }

        public async Task<string> RecognizeSpeechAsync(string audioFilePath)
        {
            if (string.IsNullOrWhiteSpace(audioFilePath))
                throw new ArgumentException("Audio file path is required.", nameof(audioFilePath));

            try
            {
                return await _speechToTextRepository.RecognizeSpeechAsync(audioFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error recognizing speech.", ex);
            }
        }
    }
}
