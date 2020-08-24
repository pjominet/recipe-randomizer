using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Identity
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Label { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
