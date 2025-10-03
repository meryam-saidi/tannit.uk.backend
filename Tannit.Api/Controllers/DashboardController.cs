using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using Tannit.Domain.Entities;
using Tannit.Infrastructure.Persistence;

namespace Tannit.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDbConnectionFactory _factory;

        public DashboardController(IDbConnectionFactory factory) { _factory = factory; }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // Récupérer l'ID de l'utilisateur depuis le token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID missing in token");

            var userId = Guid.Parse(userIdClaim.Value);

            using var connection = _factory.Create();

            // Charger toutes les transactions de l'utilisateur
            var transactions = (await connection.QueryAsync<Transaction>(
                "SELECT * FROM Transactions WHERE UserId = @UserId ORDER BY CreatedAtUtc DESC",
                new { UserId = userId }
            )).ToList();

            // Calculs
            decimal balance = transactions
                .Where(t => t.Status == "Vested")
                .Sum(t => t.Amount);

            decimal processing = transactions
                .Where(t => t.Status == "Settled")
                .Sum(t => t.Amount);

            decimal cash = transactions
                .Where(t => t.Status == "Cash")
                .Sum(t => t.Amount);

            decimal walletValue = balance + processing + cash;

            // Préparer le résultat
            var result = new
            {
                balance,
                processing,
                cash,
                walletValue,
                transactions = transactions.Select(t => new
                {
                    id = t.Id,
                    date = t.CreatedAtUtc,
                    type = t.Type,
                    status = t.Status,
                    amount = t.Amount,
                    tannit = t.Amount / 10
                })
            };

            return Ok(result);
        }
    }
}
