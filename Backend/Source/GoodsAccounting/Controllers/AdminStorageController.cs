using AutoMapper;
using GoodsAccounting.Model.DataBase;
using GoodsAccounting.Model.DTO;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.Services.DataBase;
using GoodsAccounting.Services.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <response code="200">Goods storage was updated.</response>
    /// <response code="400">Returns if unknown exception was thrown.</response>
    /// <response code="401">Returns if user not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("~/storage/set_count")]
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
}