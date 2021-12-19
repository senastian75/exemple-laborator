using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.Api.Models
{
    public class InputProducts
    {
        [Required]
        [RegularExpression(ProductCode.Pattern)]
        public string ProductID { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
