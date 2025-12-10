using Microsoft.AspNetCore.Mvc;
using Tannit.Application.Properties;

namespace Tannit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _repo;

        public PropertiesController(IPropertyRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var properties = await _repo.GetAllAsync(ct);

            var dto = properties.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                totalValue = p.TotalValue,
                totalShares = p.TotalShares,
                pricePerShare = p.PricePerShare,
                availableShares = p.AvailableShares
            });

            return Ok(dto);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var property = await _repo.GetByIdAsync(id, ct);
            if (property == null) return NotFound();

            return Ok(new
            {
                id = property.Id,
                name = property.Name,
                description = property.Description,
                totalValue = property.TotalValue,
                totalShares = property.TotalShares,
                pricePerShare = property.PricePerShare,
                availableShares = property.AvailableShares
            });
        }
    }

}
