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
        Task<List<AdvertisementsViewDto>> GetAllAdvertisements();
        Task<AdvertisementsViewDto> GetAdvertisementById(int id);
        Task UpdateAdvertisement(int id, AdvertisementUpdateDto model);
        Task DeleteAdvertisement(int id);
    }
}