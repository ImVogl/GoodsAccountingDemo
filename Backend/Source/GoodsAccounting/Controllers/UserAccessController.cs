
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GoodsAccounting.Services.Validator;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ILogger = NLog.ILogger;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.Model.DTO;
using GoodsAccounting.Services.DataBase;
using Microsoft.EntityFrameworkCore;
using GoodsAccounting.Services.Password;
using GoodsAccounting.Services.SecurityKey;
using GoodsAccounting.Model.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GoodsAccounting.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.TextConverter;

namespace GoodsAccounting.Controllers
{
    /// <summary>
    /// This controller manages to access rules.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccessController : ControllerBase
    {
        /// <summary>
        /// Default expired timespan im minutes.
        /// </summary>
        private const int ExpiredTokenTimeSpan = 60;

        /// <summary>
        /// Current class logger.
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Instance of <see cref="IDataBase"/>.
        /// </summary>
        private readonly IDataBase _db;

        /// <summary>
        /// Instance of <see cref="IPassword"/>.
        /// </summary>
        private readonly IPassword _passwordService;

        /// <summary>
        /// Instance of <see cref="IDtoValidator"/>.
        /// </summary>
        private readonly IDtoValidator _validator;

        /// <summary>
        /// Instance of <see cref="IResponseBodyBuilder"/>.
        /// </summary>
        private readonly IResponseBodyBuilder _bodyBuilder;

        /// <summary>
        /// Instance of <see cref="ISecurityKeyExtractor"/>.
        /// </summary>
        private readonly ISecurityKeyExtractor _keyExtractor;

        /// <summary>
        /// Instance of <see cref="ITextConverter"/>.
        /// </summary>
        private readonly ITextConverter _textConverter;

        /// <summary>
        /// Valid audience for token validation.
        /// </summary>
        private readonly string _validAudience;

        /// <summary>
        /// Valid issuer for token validation.
        /// </summary>
        private readonly string _validIssuer;

        /// <summary>
        /// Create new instance of <see cref="UserAccessController"/>.
        /// </summary>
        /// <param name="db">Instance of <see cref="IDataBase"/>.</param>
        /// <param name="passwordService">Instance of <see cref="IPassword"/>.</param>
        /// <param name="validator">Instance of <see cref="IDtoValidator"/>.</param>
        /// <param name="bodyBuilder">Instance of <see cref="IResponseBodyBuilder"/>.</param>
        /// <param name="keyExtractor">Instance of <see cref="ISecurityKeyExtractor"/>.</param>
        /// <param name="textConverter">Instance of <see cref="ITextConverter"/>.</param>
        /// <param name="jwtOption">Option for <see cref="JwtSection"/>.</param>
        public UserAccessController(
            IDataBase db,
            IPassword passwordService,
            IDtoValidator validator,
            IResponseBodyBuilder bodyBuilder,
            ISecurityKeyExtractor keyExtractor,
            ITextConverter textConverter,
            IOptions<JwtSection> jwtOption)
        {
            _db = db;
            _passwordService = passwordService;
            _validator = validator;
            _bodyBuilder = bodyBuilder;
            _keyExtractor = keyExtractor;
            _textConverter = textConverter;
            _validAudience = jwtOption.Value.ValidAudience ?? throw new NullReferenceException(nameof(jwtOption.Value.ValidAudience));
            _validIssuer = jwtOption.Value.ValidIssuer ?? throw new NullReferenceException(nameof(jwtOption.Value.ValidIssuer));
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="dto"><see cref="AddUserDto"/>.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [Authorize(Roles = UserRole.Administrator)]
        [HttpPost("~/add_user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddUserAsync([FromBody] AddUserDto dto)
        {
            Log.Info($"User with identifier \'{dto.SenderId}\' is trying to create new user.");
            try
            {
                if (dto.SenderId < 1 || string.IsNullOrWhiteSpace(dto.Token))
                {
                    Log.Warn("User's identifier is incorrect.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == dto.SenderId)
                    .ConfigureAwait(false);

                if (user != null)
                    return BadRequest(_bodyBuilder.EntityExistsBuild());

                const int expiredMinutes = 15;
                var expiredTime = DateOnly.FromDateTime(DateTime.Now.AddMinutes(expiredMinutes));
                var password = _passwordService.GeneratePassword();
                var (salt, hash) = _passwordService.Hash(password);
                var newUser = new User
                {
                    UserLogin = $"{_textConverter.Convert(dto.Surname)}_{_textConverter.Convert(dto.Name)}",
                    Name = dto.Name,
                    BirthDate = DateOnly.FromDateTime(dto.BirthDay),
                    Surname = dto.Surname, PasswordExpired = expiredTime, Hash = hash, Salt = salt,
                    Role = UserRole.RegisteredUser
                };

                await _db.AddUser(newUser).ConfigureAwait(false);
                var jwtToken = ProcessToken(newUser, expiredMinutes);
                return Ok(_bodyBuilder.TokenNewUserBuild("", password, jwtToken));
            }
            catch (BadPasswordException)
            {
                return Unauthorized();
            }
            catch
            {
                Log.Info("Unknown error was trown.");
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="password">New user password.</param>
        /// <param name="token">Access token.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [Authorize]
        [HttpPost("~/change/{id}/{password}/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePasswordAsync(int id, string password, string token)
        {
            Log.Info($"User with identifier \'{id}\' is trying to change treir password.");
            try {
                if (id < 1 || string.IsNullOrWhiteSpace(token)) {
                    Log.Warn("User's identifier is incorrect.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }
                
                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == id)
                    .ConfigureAwait(false);

                if (user == null)
                    return Unauthorized();
                
                var (salt, hash) =_passwordService.Hash(password);
                await _db.ChangePasswordAsync(id, salt, hash).ConfigureAwait(false);

                var jwtToken = ProcessToken(user, ExpiredTokenTimeSpan);
                return Ok(_bodyBuilder.TokenBuild(jwtToken));
            }
            catch (BadPasswordException) {
                return Unauthorized();
            }
            catch {
                Log.Info("Unknown error was trown.");
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Sign In user.
        /// </summary>
        /// <param name="dto"><see cref="SignInDto"/>.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if request pair of login-password is incorrect.</response>
        [HttpPost("~/signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignInUserAsync([FromBody] SignInDto dto)
        {
            Log.Info($"User with login \'{dto.UserLogin}\' is trying to sign in.");

            try
            {
                if (_validator.Validate(dto))
                {
                    Log.Warn("User sent invalid DTO.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.UserLogin == dto.UserLogin)
                    .ConfigureAwait(false);

                if (user == null || !_passwordService.VerifyPassword(user.Hash, dto.Password, user.Salt))
                    return Unauthorized();

                var jwtToken = ProcessToken(user, ExpiredTokenTimeSpan);
                return Ok(_bodyBuilder.TokenBuild(jwtToken));
            }
            catch (BadPasswordException) {
                return Unauthorized();
            }
            catch {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Sign Out user.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="token">Access token.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [Authorize]
        [HttpPost("~/signout/{id}/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignOutUserAsync(int id, string token)
        {
            Log.Info($"User with identifier \'{id}\' is trying to sign out.");

            try
            {
                if (id < 1 || string.IsNullOrWhiteSpace(token)) {
                    Log.Warn("User's identifier is incorrect.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == id)
                    .ConfigureAwait(false);

                if (user == null)
                    return Unauthorized();

                var jwtToken = ProcessToken(user, 0);
                return Ok(_bodyBuilder.TokenBuild(jwtToken));
            }
            catch
            {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Processing token.
        /// </summary>
        /// <param name="user"><see cref="User"/>.</param>
        /// <param name="minutes">Expired time.</param>
        /// <returns>Token.</returns>
        private string ProcessToken(User user, int minutes)
        {
            var credentials = new SigningCredentials(_keyExtractor.Extract(), SecurityAlgorithms.Sha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = CreateClaimsIdentity(user),
                Expires = DateTime.Now.AddMinutes(minutes),
                Audience = _validAudience,
                Issuer = _validIssuer,
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Create claim identity.
        /// </summary>
        /// <param name="user"><see cref="User"/>.</param>
        /// <returns><see cref="ClaimsIdentity"/>.</returns>
        private ClaimsIdentity CreateClaimsIdentity(User user)
        {
            return new ClaimsIdentity(new []
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Birthdate, user.BirthDate.ToString(CultureInfo.InvariantCulture)),
                    new Claim(JwtRegisteredClaimNames.FamilyName, user.Surname),
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserLogin),
                    new Claim(ClaimTypes.Role, user.Role)

                }
            );
        }
    }
}
