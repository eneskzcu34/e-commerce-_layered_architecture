using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.SettingsDTOs
{
    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Geçersiz isim. Sadece harfler ve boşluklar kullanılabilir.")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Geçersiz telefon numarası.")]
        public string PhoneNumber { get; set; }
        // public string Address { get; set; }
        
    }
}