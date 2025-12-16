using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tannit.Api.Services;
using Tannit.Application.Investments.DTO;
using Tannit.Infrastructure.Repositories;

namespace Tannit.Api.Controllers
{
    [ApiController]
    [Route("api/investments")]
    [Authorize]
    public class InvestmentController : ControllerBase
    {
        private readonly InvestmentService _investmentService;
        private readonly InvestmentRepository _investmentRepo;

        public InvestmentController(InvestmentService investmentService)
        {
            _investmentService = investmentService;
        }
        [HttpPost]
        public async Task<IActionResult> Invest(
       [FromBody] CreateInvestmentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var authUserId = Guid.Parse(userId);

            await _investmentService.InvestAsync(authUserId, request);

            return Ok(new { message = "Investment successful" });
        }

        [HttpGet("myInvestments")]
        public async Task<IActionResult> MyInvestments()
        {
            var userId = Guid.Parse(
                User.FindFirst("sub")!.Value
            );

            var investments = await _investmentRepo
                .GetByUserIdAsync(userId);

            return Ok(investments);
        }
    }
}
