using System;
using Microsoft.AspNetCore.Http;

namespace VillaManager.Services.Helper {


public class Media
    {
        private readonly static string _fileUploadPath = "UploadedFiles";

        public async static Task<string> saveFile(IFormFile File, string Name) 
        {
            // Ensure the "Media" folder exists in the root directory
            if (!Directory.Exists(_fileUploadPath))
            {
                Directory.CreateDirectory(_fileUploadPath);
            }
            var filePath = Path.Combine(_fileUploadPath, Name);
            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }
            return filePath;
        }
        public async static Task<string> saveFile(string file, string Name)
        {
            try{
                // Ensure the "Media" folder exists in the root directory
                if (!Directory.Exists(_fileUploadPath))
                {
                    Directory.CreateDirectory(_fileUploadPath);
                }
                var filePath = Path.Combine(_fileUploadPath, Name);

                file = file.Substring(file.IndexOf(',') + 1);
                Guid guid = Guid.NewGuid();


                await Task.Run(() => File.WriteAllBytes(filePath, Convert.FromBase64String(file)));

                return filePath;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"File deleted: {filePath}");
                }
                else
                {
                    Console.WriteLine($"File not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {filePath}. Exception: {ex.Message}");
            }
        }

    }
}