using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.AdvertisementDTOs;

namespace Application.Interfaces
{
    public interface IAdvertisementService
    {
        Task CreateAdvertisement(AdvertisementCreateDto model);
    }
}