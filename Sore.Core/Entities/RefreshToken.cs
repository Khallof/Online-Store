namespace Store.Core.Entities
{
    // ==================================================
    // RefreshToken Entity
    // Stored in database — linked to a customer
    // ==================================================
    public class RefreshToken
    {
        public int Id { get; set; }

       
        public string Token { get; set; } = null!;

       
        public DateTime ExpiresAt { get; set; }

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

     
        public DateTime? RevokedAt { get; set; }

        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        // ==================================================
        // Computed properties
        // ==================================================

     
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

       
        public bool IsRevoked => RevokedAt != null;

        
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}
