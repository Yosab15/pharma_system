using System;
using System.Collections.Generic;
using DAL.Enums;

namespace DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ResponsibleName { get; set; } = null!; // اسم المسؤل
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public EgyptianCity City { get; set; }
        public string? Notes { get; set; }
        public string? SupplyOrderImageUrl { get; set; } // أمر التوريد
        public double? Latitude { get; set; } // الموقع
        public double? Longitude { get; set; } // الموقع
        public decimal TotalPrice { get; set; }
        public decimal? Discount { get; set; } = 0;
        public decimal? FinalPrice { get; set; } = 0;
        public string PaymentType { get; set; } = "Cash";
        public int FreeUnitsReceived { get; set; } = 0;
        public DateTime OrderDate { get; set; }
        public DateTime? LastStatusUpdate { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
            = new HashSet<OrderItem>();
    }
}