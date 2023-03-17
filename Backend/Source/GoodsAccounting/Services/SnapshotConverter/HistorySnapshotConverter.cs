using AutoMapper;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;

namespace GoodsAccounting.Services.SnapshotConverter;

/// <summary>
/// Converter data base entity to snapshots collection
/// </summary>
public class HistorySnapshotConverter : ISnapshotConverter
{
    /// <summary>
    /// Instance of <see cref="IMapper"/>.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Create new instance of <see cref="HistorySnapshotConverter"/>.
    /// </summary>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    public HistorySnapshotConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public IList<ShiftSnapshotDto> Convert(IList<WorkShift> shifts, IList<GoodsItem> goods)
    {
        if (shifts.Count == 0)
            return new List<ShiftSnapshotDto>();

        var goodsDictionary = goods.ToDictionary(item => item.Id, item => item);
        var mergedDto = new ShiftSnapshotDto { Cash = 0, UserDisplayName = string.Empty, StorageItems = new List<StorageItemInfoDto>() };
        var dtoList = new List<ShiftSnapshotDto>();
        foreach (var shift in shifts)
        {
            var dto = _mapper.Map<ShiftSnapshotDto>(shift);
            dto.StorageItems = shift.GoodItemStates.Select(state => new StorageItemInfoDto
            {
                ItemId = state.Id,
                RetailPrice = state.RetailPrice,
                Sold = state.Sold,
                WholeScalePrice = state.WholeScalePrice,
                GoodsInStorage = state.GoodsInStorage,
                WriteOff = state.WriteOff,
                Receipt = state.Receipt,
                ItemCategory = goodsDictionary.ContainsKey(state.Id) ? goodsDictionary[state.Id].Category : string.Empty,
                ItemName = goodsDictionary.ContainsKey(state.Id) ? goodsDictionary[state.Id].Name : string.Empty
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
                GoodsInStorage = shift.Min(c => c.GoodsInStorage),
                ItemCategory = goodsDictionary.ContainsKey(shift.Key) ? goodsDictionary[shift.Key].Category : string.Empty,
                ItemName = goodsDictionary.ContainsKey(shift.Key) ? goodsDictionary[shift.Key].Name : string.Empty
            }).ToList();

        if (shifts.Count > 1)
            dtoList.Add(mergedDto);

        return dtoList;
    }

    /// <inheritdoc />
    public IList<ReducedSnapshotDto> ConvertReduced(IList<WorkShift> shifts, IList<GoodsItem> goods)
    {
        if (shifts.Count == 0)
            return new List<ReducedSnapshotDto>();

        var goodsDictionary = goods.ToDictionary(item => item.Id, item => item);
        var mergedDto = new ReducedSnapshotDto { Cash = 0, UserDisplayName = string.Empty, StorageItems = new List<ReducedItemInfoDto>() };
        var dtoList = new List<ReducedSnapshotDto>();
        foreach (var shift in shifts)
        {
            var dto = _mapper.Map<ReducedSnapshotDto>(shift);
            dto.StorageItems = shift.GoodItemStates
                .Select(state => new ReducedItemInfoDto
                {
                    ItemId = state.Id,
                    RetailPrice = state.RetailPrice,
                    Sold = state.Sold,
                    ItemName = goodsDictionary.ContainsKey(state.Id) ? goodsDictionary[state.Id].Name : string.Empty,
                    ItemCategory = goodsDictionary.ContainsKey(state.Id) ? goodsDictionary[state.Id].Category : string.Empty
                }).ToList();

            dtoList.Add(dto);

            mergedDto.Cash += shift.Cash;
            mergedDto.UserDisplayName += $"{shift.UserDisplayName};{Environment.NewLine}";
        }

        mergedDto.StorageItems = shifts.SelectMany(shift => shift.GoodItemStates).GroupBy(shift => shift.Id).Select(
            shift => new ReducedItemInfoDto
            {
                ItemId = shift.Key,
                Sold = shift.Aggregate(0, (sum, item) => sum + item.Sold),
                RetailPrice = shift.Average(c => c.RetailPrice),
                ItemCategory = goodsDictionary.ContainsKey(shift.Key) ? goodsDictionary[shift.Key].Category : string.Empty,
                ItemName = goodsDictionary.ContainsKey(shift.Key) ? goodsDictionary[shift.Key].Name : string.Empty
            }).ToList();

        if (shifts.Count > 1)
            dtoList.Add(mergedDto);
        
        return dtoList;
    }
}