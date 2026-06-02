

namespace Store.Core.Entities
{
    public partial class Customer
    {

        public int CustomerID { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; } 
        public string? Address { get; set; } 

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;



        public virtual ICollection<Review> Review { get; set; } =
            new List<Review>();

        public virtual ICollection<Order> Order { get; set; } =
            new List<Order>();



    }
}
