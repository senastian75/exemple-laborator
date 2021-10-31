using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedPrice(ProductCode ProductCode, Quantity Quantity, Adresa address, int price);
}
