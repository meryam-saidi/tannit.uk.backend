using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tannit.Application.Payments
{
    public interface IPaymentGateway
    {
        // crée une session checkout et retourne l'URL de redirection
        //Task<string> CreateCheckoutSessionAsync(Guid transactionId, long amountInMinorUnits, string currency, string successUrl, string cancelUrl, CancellationToken ct = default);

        Task<string> CreateCheckoutSessionAsync(decimal amount, string currency, string successUrl, string cancelUrl);

        // (facultatif) Vérifier la signature du webhook côté backend si besoin
        //bool ValidateWebhookSignature(string payload, string sigHeader, string webhookSecret);
    }

}
