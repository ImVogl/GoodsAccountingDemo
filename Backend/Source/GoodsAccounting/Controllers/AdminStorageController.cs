using AutoMapper;
using GoodsAccounting.Model;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.Services.DataBase;
using GoodsAccounting.Services.SnapshotConverter;
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
    /// Instance of <see cref="ISnapshotConverter"/>.
    /// </summary>
    private readonly ISnapshotConverter _converter;

    /// <summary>
    /// Create new instance of <see cref="AdminStorageController"/>.
    /// </summary>
    /// <param name="db">Instance of <see cref="IAdminStorageContext"/>.</param>
    /// <param name="mapper">Instance of <see cref="IMapper"/>.</param>
    /// <param name="validator">Instance of <see cref="IDtoValidator"/>.</param>
    /// <param name="bodyBuilder">Instance of <see cref="IResponseBodyBuilder"/>.</param>
    /// <param name="converter">Instance of <see cref="ISnapshotConverter"/>.</param>
    public AdminStorageController(IAdminStorageContext db, IMapper mapper, IDtoValidator validator, IResponseBodyBuilder bodyBuilder, ISnapshotConverter converter)
    {
        _db = db;
        _mapper = mapper;
        _validator = validator;
        _bodyBuilder = bodyBuilder;
        _converter = converter;
    }

    /// <summary>
    /// Make revision.
    /// </summary>
    /// <param name="dto">Information about revision.</param>
    /// <returns></returns>
    /// <response code="200">Goods storage was updated.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if user not found or hasn't access.</response>
    [HttpPost("~/storage/revision")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
    public async Task<IActionResult> StorageRevisionAsync([FromBody] GoodsRevisionDto dto)
    {
        if (_validator.Validate(dto)) {
            Log.Error($"DTO \"{typeof(GoodsRevisionDto)}\" is invalid.");
            return BadRequest(_bodyBuilder.InvalidDtoBuild());
        }

        try {
            await _db.UpdateGoodsStorageAsync(dto.Id, _mapper.Map<Dictionary<Guid, GoodsItemStateChanging>>(dto.Items)).ConfigureAwait(false);
            return Ok();
        }
        catch (TableAccessException) {
            Log.Error($"User with id \"{dto.Id}\" not found.");
            return Unauthorized();
        }
        catch (EntityNotFoundException) {
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Registry goods supply.
    /// </summary>
    /// <param name="dto">Information about supplied goods.</param>
    /// <returns></returns>
    /// <response code="200">Goods storage was updated.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if user not found or hasn't access.</response>
    [HttpPost("~/storage/supplies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
    public async Task<IActionResult> SuppliesAsync([FromBody] GoodsSuppliesDto dto)
    {
        if (_validator.Validate(dto))
        {
            Log.Error($"DTO \"{typeof(GoodsRevisionDto)}\" is invalid.");
            return BadRequest(_bodyBuilder.InvalidDtoBuild());
        }

        try {
            await _db.UpdateGoodsStorageAsync(dto.Id, _mapper.Map<Dictionary<Guid, GoodsItemStateChanging>>(dto.Items))
                .ConfigureAwait(false);
            return Ok();
        }
        catch (TableAccessException) {
            Log.Error($"User with id \"{dto.Id}\" not found.");
            return Unauthorized();
        }
        catch (EntityNotFoundException) {
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch {
            return BadRequest(_bodyBuilder.UnknownBuild());
        }
    }

    /// <summary>
    /// Editing goods list.
    /// </summary>
    /// <param name="dto">Data model for editing goods list.</param>
    /// <returns></returns>
    /// <response code="200">Goods storage was updated.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if user not found or hasn't access.</response>
    [HttpPost("~/storage/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
    public async Task<IActionResult> EditGoodsListAsync([FromBody] EditGoodsListDto dto)
    {
        if (_validator.Validate(dto))
            return BadRequest(_bodyBuilder.InvalidDtoBuild());

        try {
            if (dto.CreateNew) {
                await _db.AddNewGoodsItemAsync(dto.UserId, _mapper.Map<GoodsItem>(dto)).ConfigureAwait(false);
            } else if (dto.Remove) {

                if (dto.Id == null)
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());

                await _db.RemoveGoodsItemAsync(dto.UserId, (Guid)dto.Id).ConfigureAwait(false);
            } else if (dto.Restore) {
                if (dto.Id == null)
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());

                await _db.RestoreGoodsItemAsync(dto.UserId, (Guid)dto.Id).ConfigureAwait(false);
            }
            else {
                if (dto.Id == null)
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());

                await _db.RenameGoodsItemAsync(dto.UserId, (Guid)dto.Id, dto.Name).ConfigureAwait(false);
            }

            return Ok();

        }
        catch (EntityNotFoundException) {
            return BadRequest(_bodyBuilder.EntityNotFoundBuild());
        }
        catch (TableAccessException) {
            return Unauthorized();
        }
        catch (EntityExistsException) {
            return BadRequest(_bodyBuilder.EntityExistsBuild());
        }
        catch {
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
    [HttpGet("~/sold_statistics_full/{id}/{day}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ShiftSnapshotDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
    public async Task<IActionResult> GetDayStatistics(int id, DateTime day)
    {
        try
        {
            var goods = await _db.Goods.ToListAsync().ConfigureAwait(false);
            return Ok(_converter.Convert(await _db.GetWorkShiftSnapshotsAsync(id, DateOnly.FromDateTime(day)).ConfigureAwait(false), goods));
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
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
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