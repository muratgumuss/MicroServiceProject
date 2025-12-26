using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceApp.Order.Application.Contracts.Refit.PaymentService
{
    public record CreatePaymentResponse(Guid? PaymentId, bool Status, string? ErrorMessage);
}
