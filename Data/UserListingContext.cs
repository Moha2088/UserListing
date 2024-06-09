using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserListing.Models;

namespace UserListing.Data
{
    public class UserListingContext : DbContext
    {
        public UserListingContext (DbContextOptions<UserListingContext> options)
            : base(options)
        {
        }

        public DbSet<UserListing.Models.User> User { get; set; } = default!;
    }
}
