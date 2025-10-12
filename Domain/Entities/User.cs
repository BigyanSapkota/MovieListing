using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        //public string Username { get; set; }
  
  
        public ICollection<Movie> Movie { get; set; }
        public ICollection<Comment> Comment { get; set; }
        public ICollection<Rating> Rating { get; set; }
        public ICollection<WatchList> WatchList { get; set; } = new List<WatchList>();
        public ICollection<WaterBill> WaterBill { get; set; }
        public ICollection<ElectricityBill> ElectricityBill { get; set; }
        public ICollection<Bill> Bills { get; set; }

        public Guid? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public ICollection<PaymentTransaction> PaymentTransaction { get; set; } = new List<PaymentTransaction>();

    }




    public class Role : IdentityRole
    {
        public Role() : base() { }
               public Role(string roleName) : base(roleName) { }

    }


    //public class UserClaim : IdentityUserClaim<Guid> { }
    //public class UserLogin : IdentityUserLogin<Guid> { }
    //public class UserToken : IdentityUserToken<Guid> { }
    //public class RoleClaim : IdentityRoleClaim<Guid> { }
    //public class UserRole : IdentityUserRole<Guid> { }





}
