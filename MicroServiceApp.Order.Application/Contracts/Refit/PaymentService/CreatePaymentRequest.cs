using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceApp.Order.Application.Contracts.Refit.PaymentService
{
    public record CreatePaymentRequest(
        string OrderCode,
        string CardNumber,
        string CardHolderName,
        string CardExpirationDate,
        string CardSecurityNumber,
        decimal Amount);
}
