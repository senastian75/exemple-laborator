using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedPrice(ProductCode productCode, Quantity quantity, Adresa address, int price);
}
