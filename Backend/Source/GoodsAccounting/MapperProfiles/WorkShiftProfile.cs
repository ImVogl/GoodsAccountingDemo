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

        CreateMap<EditGoodsListDto, GoodsItem>()
            .ForMember(item => item.Actives, opt => opt.MapFrom(_ => true))
            .ForMember(item => item.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

        CreateMap<IList<RevisionGoodsItemDto>, Dictionary<Guid, GoodsItemStateChanging>>().ConstructUsing(
            (dtoList, _) =>
            {
                return dtoList.ToDictionary(item => item.Id, item => new GoodsItemStateChanging
                {
                    Category = item.Category,
                    WholeScalePrice = 0F,
                    RetailPrice = item.RetailPrice,
                    Receipt = 0,
                    Storage = item.Storage,
                    WriteOff = item.WriteOff
                });
            });

        CreateMap<WorkShift, ShiftSnapshotDto>().ForMember(dto => dto.StorageItems, opt => opt.Ignore());
        CreateMap<GoodsItem, StorageItemInfoDto>()
            .ForMember(dto => dto.ItemName, opt => opt.MapFrom(scr => scr.Name));
            
        CreateMap<IList<WorkShift>, IList<ShiftSnapshotDto>>().ConstructUsing((shifts, context) =>
        {
            if (shifts.Count == 0)
                throw new ArgumentOutOfRangeException();

            var mergedDto = new ShiftSnapshotDto { Cash = 0, UserDisplayName = string.Empty, StorageItems = new List<StorageItemInfoDto>() };
            var dtoList = new List<ShiftSnapshotDto>();
            foreach (var shift in shifts)
            {
                var dto = context.Mapper.Map<ShiftSnapshotDto>(shift);
                dto.StorageItems = shift.GoodItemStates.Select(state => new StorageItemInfoDto
                {
                    ItemId = state.Id,
                    RetailPrice = state.RetailPrice,
                    Sold = state.Sold,
                    WholeScalePrice = state.WholeScalePrice,
                    GoodsInStorage = state.GoodsInStorage,
                    WriteOff = state.WriteOff,
                    Receipt = state.Receipt
                }).ToList();

                dtoList.Add(dto);

                mergedDto.Cash += shift.Cash;
                mergedDto.UserDisplayName += $"{shift.UserDisplayName};{Environment.NewLine}";
            }

            mergedDto.StorageItems = shifts.SelectMany(shift => shift.GoodItemStates).GroupBy(shift => shift.Id).Select(
                shift => new StorageItemInfoDto
                {
                    ItemId = shift.Key,
                    Sold = shift.Aggregate(0, (sum, item) => sum + item.Sold),
                    RetailPrice = shift.Average(c => c.RetailPrice),
                    WholeScalePrice = shift.Average(c => c.WholeScalePrice),
                    Receipt = shift.Aggregate(0, (sum, item) => sum + item.Receipt),
                    WriteOff = shift.Aggregate(0, (sum, item) => sum + item.WriteOff),
                    GoodsInStorage = shift.Min(c => c.GoodsInStorage)
                }).ToList();

            dtoList.Add(mergedDto);
            return dtoList;
        });

        CreateMap<WorkShift, ReducedSnapshotDto>().ForMember(dto => dto.StorageItems, opt => opt.Ignore());
        CreateMap<IList<WorkShift>, IList<ReducedSnapshotDto>>().ConstructUsing((shifts, context) =>
        {
            if (shifts.Count == 0)
                throw new ArgumentOutOfRangeException();

            var mergedDto = new ReducedSnapshotDto { Cash = 0, UserDisplayName = string.Empty, StorageItems = new List<ReducedItemInfoDto>() };
            var dtoList = new List<ReducedSnapshotDto>();
            foreach (var shift in shifts)
            {
                var dto = context.Mapper.Map<ReducedSnapshotDto>(shift);
                dto.StorageItems = shift.GoodItemStates.Select(state => new ReducedItemInfoDto{ ItemId = state.Id, RetailPrice = state.RetailPrice, Sold = state.Sold }).ToList();
                dtoList.Add(dto);

                mergedDto.Cash += shift.Cash;
                mergedDto.UserDisplayName += $"{shift.UserDisplayName};{Environment.NewLine}";
            }

            mergedDto.StorageItems = shifts.SelectMany(shift => shift.GoodItemStates).GroupBy(shift => shift.Id).Select(
                shift => new ReducedItemInfoDto
                {
                    ItemId = shift.Key, Sold = shift.Aggregate(0, (sum, item) => sum + item.Sold), RetailPrice = shift.Average(c => c.RetailPrice)
                }).ToList();

            dtoList.Add(mergedDto);
            return dtoList;
        });
    }
}