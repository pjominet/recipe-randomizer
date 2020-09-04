namespace RecipeRandomizer.Business.Models.Identity
{
    public class LockRequest
    {
        public bool IsLocked { get; set; }
        public int? LockedById { get; set; }
    }
}
