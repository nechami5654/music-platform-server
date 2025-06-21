using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using Repository.Interface;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class SpeechToTextRepository : ISpeechToTextRepository
    {
        private readonly IConfiguration _config;
        public SpeechToTextRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> RecognizeSpeechAsync(string audioFilePath)
        {
            if (string.IsNullOrWhiteSpace(audioFilePath) || !File.Exists(audioFilePath))
                throw new ArgumentException("Invalid audio file path.", nameof(audioFilePath));

            try
            {
                var config = SpeechConfig.FromSubscription(
                    _config["AzureSpeech:SubscriptionKey"],
                    _config["AzureSpeech:Region"]
                );

                config.SpeechRecognitionLanguage = "he-IL";

                config.SetProperty("SPEECH-NoiseSuppression", "High");

                if (Path.GetExtension(audioFilePath).ToLower() == ".mp3")
                {
                    audioFilePath = ConvertMp3ToWav(audioFilePath); 
                }

                using var audioConfig = AudioConfig.FromWavFileInput(audioFilePath);

                using var recognizer = new SpeechRecognizer(config, audioConfig);

                var recognizedText = new StringBuilder();
                var stopRecognition = new TaskCompletionSource<int>();

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        recognizedText.Append(e.Result.Text + " "); 
                    }
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    stopRecognition.TrySetResult(0);
                };

                await recognizer.StartContinuousRecognitionAsync();

                double durationMs =GetAudioDuration(audioFilePath); 

                await Task.Delay((int)durationMs + 1000);

                await recognizer.StopContinuousRecognitionAsync();

                return recognizedText.ToString().Trim() ?? "No speech recognized";
            }
            catch (Exception ex)
            {
                throw new Exception("Error recognizing speech.", ex);
            }
        }

        public double GetAudioDuration(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException("Invalid file path.", nameof(filePath));

            try
            {
                using var reader = new WaveFileReader(filePath);
                return reader.TotalTime.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting audio duration.", ex);
            }
        }

        public string ConvertMp3ToWav(string mp3FilePath)
        {
            if (string.IsNullOrWhiteSpace(mp3FilePath) || !File.Exists(mp3FilePath))
                throw new ArgumentException("Invalid MP3 file path.", nameof(mp3FilePath));

            try
            {
                string wavFilePath = Path.ChangeExtension(mp3FilePath, ".wav");

                // קריאת קובץ ה-MP3 והמרתו לפורמט WAV
                using var reader = new Mp3FileReader(mp3FilePath);
                using var writer = new WaveFileWriter(wavFilePath, reader.WaveFormat);
                reader.CopyTo(writer); // העתקת תוכן הקובץ לפורמט WAV

                return wavFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception("Error converting MP3 to WAV.", ex);
            }
        }
    }
}
