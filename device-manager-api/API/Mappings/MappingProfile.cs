using AutoMapper;
using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Domain.Entities;

namespace device_manager_api.API.Mappings;

/// <summary>
/// AutoMapper profile for device mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Device to DeviceResponse
        CreateMap<Device, DeviceResponse>();

        // CreateDeviceRequest to Device
        CreateMap<CreateDeviceRequest, Device>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

        // UpdateDeviceRequest to Device
        CreateMap<UpdateDeviceRequest, Device>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

        // Device to DevicePatchDto
        CreateMap<Device, DevicePatchDto>();
    }
}
