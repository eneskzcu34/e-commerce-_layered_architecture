using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.AdvertisementDTOs
{
    public class AdvertisementCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
    }
}