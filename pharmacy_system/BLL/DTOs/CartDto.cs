using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class CartDto
    {
        public int CartId { get; set; }

        public decimal TotalPrice { get; set; }

        public List<CartItemDto> Items { get; set; }
            = new();
    }
}
