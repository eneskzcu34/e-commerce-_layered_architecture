using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using E_Shopping.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.DTOs.UserDTOs
{
    public class UserEditDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public Address? Address { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RoleId { get; set; }
        public SelectList? SelectLists { get; set; }
    }
}