using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;

namespace Exemple.Domain.Repositories
{
    public interface ICosRepository
    {
        TryAsync<Unit> TrySaveCos(PublishedCarucior paidCarucior);
    }
}
