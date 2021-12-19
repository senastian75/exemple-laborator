using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Data.Models
{
    public class ProductDto
    {
        public string   ProductID { get; set; }
        public int      Stock     { get; set; }
        public int      Price { get; set; }
    }
}
