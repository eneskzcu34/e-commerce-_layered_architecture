using System.ComponentModel.DataAnnotations;


namespace E_Shopping.Application.DTOs.AccountDTOs
{
    public class RegisterDTo
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli email giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar zorunludur")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmPassword { get; set; }
    }
}