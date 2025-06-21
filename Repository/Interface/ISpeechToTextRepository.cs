using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ISpeechToTextRepository
    {
        Task<string> RecognizeSpeechAsync(string audioFilePath);
        double GetAudioDuration(string filePath);
        string ConvertMp3ToWav(string mp3FilePath);
    }
}
