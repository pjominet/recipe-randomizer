namespace RecipeRandomizer.Business.Models.Identity
{
    public class LockRequest
    {
        public bool UserLock { get; set; }
        public int? LockedById { get; set; }
    }
}
