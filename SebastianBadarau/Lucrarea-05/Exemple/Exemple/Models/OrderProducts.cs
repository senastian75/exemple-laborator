using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record OrderProducts(int id, string OrderID ,string ProductID, int Quantity, int Price, string Address);

}
