using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.AdressesDTOs
{
    public class AdressesAllDto
    {
        public AdressesCreateDto CreateDto { get; set; }
        public AdressesViewDto ViewDto { get; set; }
        public AdressesUpdateDto UpdateDto { get; set; }
    }
}