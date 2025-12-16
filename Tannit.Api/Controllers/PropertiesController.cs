using Microsoft.AspNetCore.Mvc;
using Stripe;
using Tannit.Application.Properties;
using Tannit.Domain.Entities;

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
        /*
        [HttpGet]
        public async Task<IActionResult> GetAll1(CancellationToken ct)
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
                availableShares = p.AvailableShares,
                address = p.Address,
                latitude = p.Latitude,
                longitude = p.Longitude,
                lotNumber = p.LotNumber,
                lotArea = p.LotArea,
                gesEmission = p.GESEmission,
                energyConsumption = p.EnergyConsumption,
                energyClass = p.EnergyClass,
            });

            return Ok(dto);
        }
        */
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var properties = await _repo.GetAllAsync(ct);

            var result = properties.Select(p => new Property
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                TotalValue = p.TotalValue,
                TotalShares = p.TotalShares,
                AvailableShares = p.AvailableShares,
                //PricePerShare = p.PricePerShare,
                Address = p.Address,
                EnergyConsumption = p.EnergyConsumption,
                //EnergyClass = p.EnergyClass,
                LotNumber = p.LotNumber,
                LotArea = p.LotArea,
                Images = p.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new PropertyImage
                    {
                        ImageUrl = i.ImageUrl,
                        IsCover = i.IsCover,
                        DisplayOrder = i.DisplayOrder
                    }).ToList()
            });

            return Ok(result);
        }
        /*
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById1(Guid id, CancellationToken ct)
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
                availableShares = property.AvailableShares,
                address = property.Address,
                latitude = property.Latitude,
                longitude = property.Longitude,
                lotNumber = property.LotNumber,
                lotArea = property.LotArea,
                gesEmission = property.GESEmission,
                energyConsumption = property.EnergyConsumption,
                energyClass = property.EnergyClass,
            });
        }
        */
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            if (p == null) return NotFound();

            return Ok(new Property
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                TotalValue = p.TotalValue,
                TotalShares = p.TotalShares,
                AvailableShares = p.AvailableShares,
                //PricePerShare = p.PricePerShare,
                Address = p.Address,
                EnergyConsumption = p.EnergyConsumption,
                //EnergyClass = p.EnergyClass,
                LotNumber = p.LotNumber,
                LotArea = p.LotArea,
                Images = p.Images.Select(i => new PropertyImage
                {
                    ImageUrl = i.ImageUrl,
                    IsCover = i.IsCover,
                    DisplayOrder = i.DisplayOrder
                }).ToList()
            });
        }
    }
}


