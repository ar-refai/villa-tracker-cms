using Microsoft.AspNetCore.Mvc;
using VillaManager.Domain.DTOs.VillaDTO;
using VillaManager.Services.Interfaces;

namespace VillaManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaController : ControllerBase
    {
        private readonly IVillaService _villaService;

        public VillaController(IVillaService villaService)
        {
            _villaService = villaService;
        }

        // GET: api/villa
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var villas = await _villaService.GetAllAsync();
            return Ok(villas);
        }

        // GET: api/villa/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var villa = await _villaService.GetByIdAsync(id);
            return villa is null ? NotFound() : Ok(villa);
        }

        // POST: api/villa
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] VillaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdVilla = await _villaService.CreateAsync(dto);
            return Ok(createdVilla);
        }

        // PUT: api/villa/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] VillaUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedVilla = await _villaService.UpdateAsync(id, dto);
            return updatedVilla ? Ok() : NotFound();
        }

        // DELETE: api/villa/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _villaService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
