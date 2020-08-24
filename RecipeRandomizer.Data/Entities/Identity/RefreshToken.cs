using System;
using Microsoft.EntityFrameworkCore;

namespace RecipeRandomizer.Data.Entities.Identity
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;

        public virtual User User { get; set; }
    }
}
