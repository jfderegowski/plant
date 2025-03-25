using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fefek5.Common.Runtime.Extensions;

namespace fefek5.Systems.SaveSystem
{
    public static class SaveSystemHelpers
    {
        public static async Task<string> GetUniqueFileName(string folderPath) =>
            await Task.Run(() =>
            {
                var filesNames = GetSaveFiles(folderPath).Select(f => f.Name).ToList();

                var fileName = Guid.NewGuid().ToString();

                while (filesNames.Contains(fileName))
                    fileName = Guid.NewGuid().ToString();

                return fileName;
            });
        
        public static FileInfo[] GetSaveFiles(string folderPath)
        {
            return Directory.Exists(folderPath)
                ? new DirectoryInfo(folderPath).GetFiles("*.sav", SearchOption.AllDirectories)
                : null;
        }

        public static void DeleteExcessFiles(string folderPath, int fileLimit)
        {
            if (fileLimit <= 0) return;

            var saveFiles = GetSaveFiles(folderPath).SortOldestFirst().ToArray();
            var filesToDelete = saveFiles.Length - fileLimit;

            for (var i = 0; i < filesToDelete; i++)
                saveFiles[i].Delete();
        }
    }
}