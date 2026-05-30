
using System.ComponentModel.DataAnnotations;
using E_Shopping.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address? Address { get; set; }

    }
}