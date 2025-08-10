using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using VillaManager.Client.Models;

namespace VillaManager.Client.Services
{
    public class VillaApiService
    {
        private readonly HttpClient _http;

        public VillaApiService(HttpClient http)
        {
            _http = http;
        }

        // -------------------------------
        // CREATE (POST)
        // -------------------------------
        public async Task<HttpResponseMessage> CreateAsync(VillaCreateDto dto)
        {
            var content = await BuildMultipartContent(dto);
            return await _http.PostAsync("api/villa", content);
        }

        // -------------------------------
        // UPDATE (PUT)
        // -------------------------------
        public async Task<HttpResponseMessage> UpdateAsync(int id, VillaUpdateDto dto)
        {
            var content = await BuildMultipartContent(dto);
            return await _http.PutAsync($"api/villa/{id}", content);
        }
        // -------------------------------
        // GET all
        // -------------------------------
        public async Task<List<VillaDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<VillaDto>>("api/villa");
        }

        // -------------------------------
        // GET by Id
        // -------------------------------
        public async Task<VillaDto?> GetAsync(int id)
        {
            return await _http.GetFromJsonAsync<VillaDto>($"api/villa/{id}");
        }

        // -------------------------------
        // DELETE
        // -------------------------------
        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"api/villa/{id}");
        }

        // -------------------------------
        // HELPER — Build Multipart
        // -------------------------------
        private async Task<MultipartFormDataContent> BuildMultipartContent(object dto)
        {
            var form = new MultipartFormDataContent();

            // Convert dto properties to form-data
            foreach (var prop in dto.GetType().GetProperties())
            {
                var value = prop.GetValue(dto);

                // Handle file uploads
                if (value is List<IBrowserFile> files)
                {
                    foreach (var file in files)
                    {
                        using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024); // 20MB
                        var ms = new MemoryStream();
                        await stream.CopyToAsync(ms);
                        var byteContent = new ByteArrayContent(ms.ToArray());
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                        form.Add(byteContent, prop.Name, file.Name);
                    }
                }
                else if (value != null)
                {
                    form.Add(new StringContent(value.ToString() ?? ""), prop.Name);
                }
            }

            return form;
        }
          
    }
}
