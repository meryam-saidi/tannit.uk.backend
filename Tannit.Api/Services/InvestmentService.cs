using System.Threading;
using Tannit.Application.Accounts;
using Tannit.Application.Investments;
using Tannit.Application.Investments.DTO;
using Tannit.Application.Properties;
using Tannit.Domain.Entities;

namespace Tannit.Api.Services
{
    public class InvestmentService
    {
        private readonly IInvestmentRepository _investmentRepo;
        private readonly IUserAccountRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;

        public InvestmentService(
            IInvestmentRepository investmentRepo,
            IUserAccountRepository userRepo,
            IPropertyRepository propertyRepo)
        {
            _investmentRepo = investmentRepo;
            _userRepository = userRepo;
            _propertyRepository = propertyRepo;
        }

        public async Task InvestAsync(Guid userId, CreateInvestmentRequest request)
        {
            var user = await _userRepository.GetByAuthUserIdAsync(userId);
            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

            if (user == null)
                throw new Exception("User not found");

            if (property == null)
                throw new Exception("Property not found");

            if (request.TannitPoints <= 0)
                throw new Exception("Invalid amount");

            if (user.Balance < request.TannitPoints)
                throw new Exception("Insufficient balance");

            if (property.AvailableShares < request.TannitPoints)
                throw new Exception("Not enough shares");

            var investment = new Investment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PropertyId = property.Id,
                TannitPoints = request.TannitPoints,
                Status = "Active"
            };

            // 1️⃣ Créer l’investissement
            await _investmentRepo.CreateAsync(investment);

            // 2️⃣ Mettre à jour l’utilisateur
            await _userRepository.DecreaseBalanceAsync(userId, request.TannitPoints);

            // 3️⃣ Mettre à jour l'état métier de la propriété
            property.AvailableShares -= request.TannitPoints;

            // Sécurité métier (optionnelle mais recommandée)
            if (property.AvailableShares < 0)
                throw new Exception("Property shares inconsistency");

            // 4️⃣ Persister la propriété
            await _propertyRepository.UpdateAsync(property);

        }
    }

}
