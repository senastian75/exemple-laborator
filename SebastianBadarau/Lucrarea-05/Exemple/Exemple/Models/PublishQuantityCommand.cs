using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record PublishQuantityCommand
    {
        public PublishQuantityCommand(IReadOnlyCollection<UnvalidatedProductQuantity> inputQuantity)
        {
            InputQuantity = inputQuantity;
        }

        public IReadOnlyCollection<UnvalidatedProductQuantity> InputQuantity { get; }
    }
}
