using System;

namespace Mamey.Ntrada.Samples.Services.Orders.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Meta { get; set; }
    }
}