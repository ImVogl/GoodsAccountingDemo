using AutoMapper;
using GoodsAccounting.Model.DTO;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.Services.DataBase;
using GoodsAccounting.Services.SnapshotConverter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using ILogger = NLog.ILogger;

namespace GoodsAccounting.Controllers;

/// <summary>
/// This controller manages to main goods actions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    /// <summary>
    /// Logger for current controller.
    /// </summary>
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Implementation of <see cref="IStorageContext"/>.
    /// </summary>
    private readonly IStorageContext _db;

    /// <summary>
    /// Implementation of <see cref="IResponseBodyBuilder"/>.
    /// </summary>
    private readonly IResponseBodyBuilder _bodyBuilder;

    /// <summary>
    /// AutoMapper mapper instance.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Implementation of <see cref="ISnapshotConverter"/>.
    /// </summary>
    private readonly ISnapshotConverter _converter;

    /// <summary>
    /// Create new instance of <see cref="StorageController"/>.
    /// </summary>
    /// <param name="db">Implementation of <see cref="IStorageContext"/>.</param>
    /// <param name="bodyBuilder">Implementation of <see cref="IResponseBodyBuilder"/>.</param>
    /// <param name="mapper">AutoMapper mapper instance.</param>
    /// <param name="converter">Implementation of <see cref="ISnapshotConverter"/>.</param>
    public StorageController(IStorageContext db, IResponseBodyBuilder bodyBuilder, IMapper mapper, ISnapshotConverter converter)
    {
        _db = db;
        _bodyBuilder = bodyBuilder;
        _mapper = mapper;
        _converter = converter;
    }

    /// <summary>
    /// Request all goods with prices.
    /// </summary>
    /// <returns><see cref="Task"/> with goods-prices list.</returns>
    /// <response code="200">Response with goods.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    [AllowAnonymous]
    [HttpGet("~/goods")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<GoodsItemDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    public Task<IActionResult> GetAllGoods()
    {
        Log.Info("Request all goods");

        try {
            return Task.FromResult<IActionResult>(Ok(_mapper.Map<IList<GoodsItemDto>>(_db.Goods.ToList())));
        }
        catch(Exception exception) {
            Log.Error(exception);
            return Task.FromResult<IActionResult>(BadRequest(_bodyBuilder.UnknownBuild()));
        }
    }

    /// <summary>
    /// Closing current working shift.
    /// </summary>
    /// <param name="id">User's identifier.</param>
    /// <param name="cash">Cash in cash machine.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Working shift was closed.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if token expired.</response>
    [HttpPost("~/close/{id}/{cash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CloseWorkingShift(int id, int cash)
    {
        try {
            await _db.CloseWorkShiftAsync(id, cash).ConfigureAwait(false);
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

    /// <summary>
    /// Initialize new working shift.
    /// </summary>
    /// <param name="id">User's identifier.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Working shift was initialized.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if token expired.</response>
    [HttpPost("~/init_shift/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InitWorkingShiftAsync(int id)
    {
        try {
            await _db.InitWorkShiftAsync(id).ConfigureAwait(false);
            return Ok();
        }
        catch (EntityExistsException) {
            Log.Error("User tryed to create new working shift, but shift already exists!");
            return BadRequest(_bodyBuilder.EntityExistsBuild());
        }
        catch (EntityNotFoundException) {
            Log.Error($"Can't find user with identifier {id}!");
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch (Exception exception){
            Log.Error(exception);
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Posting collection with sold goods count.
    /// </summary>
    /// <param name="dto"><see cref="SoldGoodsDto"/></param>
    /// <returns><see cref="Task"/></returns>
    /// <response code="200">Data was saved.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if token expired.</response>
    [HttpPost("~/sold_goods")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostSoldGoodsAsync([FromBody] SoldGoodsDto dto)
    {
        if (dto?.Sold == null) {
            Log.Error("Posting sold goods. Invalid dto");
            return BadRequest(_bodyBuilder.InvalidDtoBuild());
        }

        try {
            await _db.UpdateSoldGoodsAsync(dto.Id, dto.Sold).ConfigureAwait(false);
            return Ok();
        }
        catch (EntityNotFoundException) {
            Log.Error("Today's working shift didn't find!");
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        } 
    }

    /// <summary>
    /// Getting days when user had working shifts.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Response data saved.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if token expired.</response>
    [HttpGet("~/shift_days/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<DateTime>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWorkShiftDays(int id)
    {
        try
        {
            return Ok(await _db.WorkShifts.Where(shift => shift.UserId == id)
                .Select(shift => shift.IsOpened ? shift.OpenTime : shift.CloseTime.Date).Distinct().ToListAsync()
                .ConfigureAwait(false));
        }
        catch
        {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Getting day sold statistics.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="day">Day for requested statistics.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <response code="200">Response data saved.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if token expired.</response>
    [HttpGet("~/sold_statistics/{id}/{day}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ReducedSnapshotDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDayStatistics(int id, DateTime day)
    {
        try {
            var goods = await _db.Goods.ToListAsync().ConfigureAwait(false);
            return Ok(_converter.ConvertReduced(await _db.GetWorkShiftSnapshotsAsync(id, DateOnly.FromDateTime(day)).ConfigureAwait(false), goods));
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }
}