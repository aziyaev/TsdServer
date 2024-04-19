using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsdServer
{
    public class Product
    {
        [Key]
        public string? Code { get; set; }
        public int Quantity { get; set; }
    }
}
