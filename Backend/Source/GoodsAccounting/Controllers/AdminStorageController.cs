using AutoMapper;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.Services.DataBase;
using GoodsAccounting.Services.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using ILogger = NLog.ILogger;

namespace GoodsAccounting.Controllers;

/// <summary>
/// This controller manages to advance goods actions.
/// </summary>
[Authorize(Roles = UserRole.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class AdminStorageController : ControllerBase
{
    /// <summary>
    /// Logger.
    /// </summary>
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Instance of <see cref="IAdminStorageContext"/>.
    /// </summary>
    private readonly IAdminStorageContext _db;

    /// <summary>
    /// Instance of <see cref="IMapper"/>.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Instance of <see cref="IDtoValidator"/>.
    /// </summary>
    private readonly IDtoValidator _validator;

    /// <summary>
    /// Instance of <see cref="IResponseBodyBuilder"/>.
    /// </summary>
    private readonly IResponseBodyBuilder _bodyBuilder;

    /// <summary>
    /// Create new instance of <see cref="AdminStorageController"/>.
    /// </summary>
    /// <param name="db">Instance of <see cref="IAdminStorageContext"/>.</param>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    /// <param name="validator">Instance of <see cref="IDtoValidator"/>.</param>
    /// <param name="bodyBuilder">Instance of <see cref="IResponseBodyBuilder"/>.</param>
    public AdminStorageController(IAdminStorageContext db, IMapper mapper, IDtoValidator validator, IResponseBodyBuilder bodyBuilder)
    {
        _db = db;
        _mapper = mapper;
        _validator = validator;
        _bodyBuilder = bodyBuilder;
    }

    /// <summary>
    /// Updating goods storage state.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <response code="200">Goods storage was updated.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if user not found.</response>
    [HttpPost("~/storage/set_count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateGoodsInStorageAsync([FromBody] UpdatingGoodsDto dto)
    {
        if (_validator.Validate(dto)) {
            Log.Error($"DTO \"{typeof(UpdatingGoodsDto)}\" is invalid.");
            return BadRequest(_bodyBuilder.InvalidDtoBuild());
        }

        try {
            await _db.UpdateGoodsStorageAsync(dto.Id, dto.ItemsToRemove, _mapper.Map<List<GoodsItem>>(dto.Items)).ConfigureAwait(false);
            return Ok();
        }
        catch (EntityNotFoundException) {
            Log.Error($"User with id \"{dto.Id}\" not found.");
            return Unauthorized();
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Getting day sold statistics.
    /// </summary>
    /// <param name="day">Day for requested statistics.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Response data saved.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    [HttpGet("~/sold_statistics/{day}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDayStatistics(DateTime day)
    {
        try
        {
            var goods = _db.Goods.Where(item => item.Actives).ToDictionary(item => item.Id, item => item);
            var snapshot = _mapper.Map<WorkShiftSnapshotDto>(await _db.GetWorkShiftSnapshotsAsync(-1, DateOnly.FromDateTime(day)).ConfigureAwait(false));
            foreach (var item in snapshot.StorageItems)
            {
                item.RetailPrice = goods.ContainsKey(item.ItemId) ? goods[item.ItemId].RetailPrice : 0;
                item.WholeScalePrice = goods.ContainsKey(item.ItemId) ? goods[item.ItemId].WholeScalePrice : 0;
                item.ItemName = goods.ContainsKey(item.ItemId) ? goods[item.ItemId].Name : string.Empty;
            }

            return Ok(snapshot);
        }
        catch
        {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Closing current working shift.
    /// </summary>
    /// <param name="targetUserId">User's identifier witch shift should be closed.</param>
    /// <param name="cash">Cash in cash machine.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Working shift was closed.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    [HttpGet("~/close_other/{targetUserId}/{cash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CloseWorkShiftForTargetUserAsync(int targetUserId, int cash)
    {
        var opened = await _db.WorkShifts.AnyAsync(shift => shift.IsOpened && shift.UserId == targetUserId).ConfigureAwait(false);
        if (!opened)
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());

        try {
            await _db.CloseWorkShiftAsync(targetUserId, cash).ConfigureAwait(false);
            return Ok();
        }
        catch (EntityNotFoundException) {
            Log.Error("Todays working shift didn't find.");
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch (InvalidOperationException) {
            Log.Error("Table contains many opened working shift.");
            return BadRequest(_bodyBuilder.EntityExistsBuild());
        }
        catch (ArgumentException) {
            return BadRequest(_bodyBuilder.InvalidDtoBuild());
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }
}