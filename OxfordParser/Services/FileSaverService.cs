using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OxfordParser.Services
{
    public class FileStorageService
    {
        private readonly FileStorageOptions _options;
        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="voiceType"></param>
        /// <param name="path"></param>
        /// <returns>Путь к файлу относительно рутовой директории из настроек</returns>
        public bool TryGetExistedSoundPath(string word, VoiceType voiceType, out string path)
        {
            path = string.Empty;

            var directoryPath = GetSoundDirectoryPath(word);
            var absolutePath = GetSoundFilePath(directoryPath, word, voiceType);
            if (File.Exists(absolutePath))
            {
                path = Path.GetRelativePath(_options.SoundsRootPath, absolutePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="file"></param>
        /// <param name="voiceType"></param>
        /// <returns>Путь к файлу относительно рутовой директории из настроек</returns>
        public async Task<string> SaveSoundAsync(string word, Stream file, VoiceType voiceType)
        {
            var directoryPath = GetSoundDirectoryPath(word);
            Directory.CreateDirectory(directoryPath);

            var filePath = GetSoundFilePath(directoryPath, word, voiceType);
            if (File.Exists(filePath))
                return Path.GetRelativePath(_options.SoundsRootPath, filePath);

            using var fs = new FileStream(filePath, FileMode.CreateNew);
            await file.CopyToAsync(fs);

            return Path.GetRelativePath(_options.SoundsRootPath, filePath);
        }

        private string GetSoundDirectoryPath(string word) =>
            Path.Combine(_options.SoundsRootPath, word[0].ToString(), word.Trim().ToLower());

        private string GetSoundFilePath(string directoryPath, string word, VoiceType voiceType)
        {
            var fileName = word + GetPostfix(voiceType) + ".mp3";
            return Path.Combine(directoryPath, fileName);
        }

        private string GetPostfix(VoiceType voiceType)
        {
            switch (voiceType)
            {
                case VoiceType.US:
                    return "_us";
                case VoiceType.UK:
                    return "_uk";
                default:
                    throw new Exception();
            }
        }
    }

    public enum VoiceType
    {
        /// <summary>
        /// American
        /// </summary>
        US,
        /// <summary>
        /// United Kingdom
        /// </summary>
        UK
    }
}
