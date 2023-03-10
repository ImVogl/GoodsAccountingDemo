using AutoMapper;
using GoodsAccounting.Model;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;

namespace GoodsAccounting.MapperProfiles;

/// <summary>
/// It's AutoMapper profile for db working shifts collection and Working shift DTO.
/// </summary>
public class WorkShiftProfile : Profile
{
    /// <summary>
    /// Mapping constructor.
    /// </summary>
    public WorkShiftProfile()
    {
        CreateMap<GoodsItemDto, GoodsItem>()
            .ForMember(item => item.Storage, opt => opt.Ignore())
            .ForMember(item => item.WholeScalePrice, opt => opt.Ignore());

        CreateMap<GoodsItem, GoodsItemDto>().ForMember(item => item.Price, opt => opt.MapFrom(scr => scr.RetailPrice));
        CreateMap<EditGoodsListDto, GoodsItem>()
            .ForMember(item => item.Actives, opt => opt.MapFrom(_ => true))
            .ForMember(item => item.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

        CreateMap<RevisionGoodsItemDto, KeyValuePair<Guid, GoodsItemStateChanging>>()
            .ConstructUsing((dto, _) => new KeyValuePair<Guid, GoodsItemStateChanging>(dto.Id, new GoodsItemStateChanging
            {
                Category = dto.Category,
                WholeScalePrice = 0F,
                RetailPrice = dto.RetailPrice,
                Receipt = 0,
                Storage = dto.Storage,
                WriteOff = dto.WriteOff
            }));

        CreateMap<WorkShift, ShiftSnapshotDto>().ForMember(dto => dto.StorageItems, opt => opt.Ignore());
        CreateMap<GoodsItem, StorageItemInfoDto>()
            .ForMember(dto => dto.ItemName, opt => opt.MapFrom(scr => scr.Name));

        CreateMap<WorkShift, ReducedSnapshotDto>().ForMember(dto => dto.StorageItems, opt => opt.Ignore());
    }
}