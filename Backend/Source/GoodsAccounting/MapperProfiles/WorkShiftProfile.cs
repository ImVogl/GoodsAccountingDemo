using AutoMapper;
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
        CreateMap<WorkShift, WorkShiftSnapshotDto>().ForMember(dto => dto.StorageItems, opt => opt.Ignore());
        CreateMap<GoodsItem, StorageItemInfoDto>()
            .ForMember(dto => dto.ItemName, opt => opt.MapFrom(scr => scr.Name));
            
        CreateMap<IList<WorkShift>, WorkShiftSnapshotDto>().ConstructUsing((shifts, context) =>
        {
            if (shifts.Count == 0)
                throw new ArgumentOutOfRangeException();

            var first = shifts.First();
            var dto = context.Mapper.Map<WorkShiftSnapshotDto>(first);
            var states = shifts.SelectMany(shift => shift.GoodItemStates).GroupBy(item => item.Id);
            dto.StorageItems = states.Select(state => new StorageItemInfoDto
            {
                ItemId = state.Key,
                Receipt = state.Aggregate(0, (sum, s) => sum + s.Receipt),
                Sold = state.Aggregate(0, (sum, s) => sum + s.Sold),
                WriteOff = state.Aggregate(0, (sum, s) => sum + s.WriteOff)
            }).ToList();

            return dto;
        });
    }
}