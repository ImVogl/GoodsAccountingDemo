﻿
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
using GoodsAccounting.Services;
using GoodsAccounting.Services.TextConverter;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
        private const int ExpiredTokenTimeSpan = 15;

        /// <summary>
        /// Current class logger.
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Instance of <see cref="IUsersContext"/>.
        /// </summary>
        private readonly IUsersContext _db;

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
        /// <param name="db">Instance of <see cref="IUsersContext"/>.</param>
        /// <param name="passwordService">Instance of <see cref="IPassword"/>.</param>
        /// <param name="validator">Instance of <see cref="IDtoValidator"/>.</param>
        /// <param name="bodyBuilder">Instance of <see cref="IResponseBodyBuilder"/>.</param>
        /// <param name="keyExtractor">Instance of <see cref="ISecurityKeyExtractor"/>.</param>
        /// <param name="textConverter">Instance of <see cref="ITextConverter"/>.</param>
        /// <param name="jwtOption">Option for <see cref="BearerSection"/>.</param>
        public UserAccessController(
            IUsersContext db,
            IPassword passwordService,
            IDtoValidator validator,
            IResponseBodyBuilder bodyBuilder,
            ISecurityKeyExtractor keyExtractor,
            ITextConverter textConverter,
            IOptions<BearerSection> jwtOption)
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
        /// Update token.
        /// </summary>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [HttpGet("~/update_token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> UpdateTokenAsync()
        {
            try {
                var id = Utils.ExtractUserIdentifierFromToken(HttpContext);
                if (id == null)
                {
                    Log.Warn("Can't extract identifier from token.");
                    return Unauthorized();
                }

                var castId = (int)id;
                Log.Info($"User with identifier \'{castId}\' is trying to access token.");
                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == castId)
                    .ConfigureAwait(false);

                if (user == null)
                    return Unauthorized();

                return Ok(await AuthenticateAsync(user).ConfigureAwait(false));
            }
            catch {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Update token.
        /// </summary>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user is drinker.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [HttpPost("~/update_user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> UpdateUserInfoAsync()
        {
            try
            {
                var id = Utils.ExtractUserIdentifierFromToken(HttpContext);
                if (id == null)
                {
                    Log.Warn("Can't extract identifier from token.");
                    return Unauthorized();
                }

                var castId = (int)id;
                Log.Info($"User with identifier \'{castId}\' is trying to information.");
                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == castId)
                    .ConfigureAwait(false);

                if (user == null)
                    return Unauthorized();
                var responseData = new UserInfoDto
                {
                    UserId = user.Id,
                    IsAdmin = user.Role == UserRole.Administrator,
                    ShiftIsOpened = await _db.GetWorkingShiftStateAsync(user.Id).ConfigureAwait(false),
                    UserDisplayedName = $"{user.Name} {user.Surname}",
                    AssessToken = string.Empty
                };
                return Ok(responseData);
            }
            catch
            {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Getting all user.
        /// </summary>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that adding is success.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find or password is invalid.</response>
        [Authorize(Roles = UserRole.Administrator)]
        [HttpGet("~/all_users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<UserLoginDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> GetAllLogins()
        {
            Log.Info(@"User requested all users list");
            try
            {
                var logins = await _db.Users
                    .Where(user => user.Role == UserRole.RegisteredUser)
                    .Select(user => new UserLoginDto { Id = user.Id, Login = user.UserLogin })
                    .ToListAsync()
                    .ConfigureAwait(false);

                return Ok(logins);
            }
            catch (Exception exception) {
                Log.Error(exception);
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Adding new user.
        /// </summary>
        /// <param name="dto"><see cref="AddUserDto"/>.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that adding is success.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find or password is invalid.</response>
        [Authorize(Roles = UserRole.Administrator)]
        [HttpPost("~/add_user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewUserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> AddUserAsync([FromBody] AddUserDto dto)
        {
            Log.Info($"User with identifier \'{dto.SenderId}\' is trying to create new user.");
            try {
                if (dto.SenderId < 1)
                {
                    Log.Warn("User's identifier is incorrect.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var login = $"{_textConverter.Convert(dto.Surname)}_{_textConverter.Convert(dto.Name)}";
                var birthDay = DateOnly.FromDateTime(dto.BirthDay);
                if (await _db.DoesUserExistsAsync(login, dto.Name, dto.Surname, birthDay).ConfigureAwait(false))
                    return BadRequest(_bodyBuilder.EntityExistsBuild());

                const int expiredMinutes = 15;
                var expiredTime = DateTime.Now.AddMinutes(expiredMinutes);
                var password = _passwordService.GeneratePassword();
                var (salt, hash) = _passwordService.Hash(password);
                var newUser = new User
                {
                    UserLogin = login,
                    Name = dto.Name,
                    BirthDate = birthDay,
                    Surname = dto.Surname, PasswordExpired = expiredTime, Hash = hash, Salt = salt,
                    Role = UserRole.RegisteredUser
                };

                await _db.AddUserAsync(newUser).ConfigureAwait(false);
                var newUserDto = new NewUserDto
                {
                    Login = login,
                    Password = password
                };

                return Ok(newUserDto);
            }
            catch (BadPasswordException) {
                return Unauthorized();
            }
            catch (EntityExistsException) {
                Log.Error("User with the same parameters exists.");
                return BadRequest(_bodyBuilder.EntityExistsBuild());
            }
            catch {
                Log.Info("Unknown error was trown.");
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Remove user.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that removing is success.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if token expired.</response>
        [Authorize(Roles = UserRole.Administrator)]
        [HttpDelete("~/remove_user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveUserAsync(int id)
        {
            if (!await _db.Users.AnyAsync(user => user.Id == id).ConfigureAwait(false))
                return BadRequest(_bodyBuilder.EntityNotFoundBuild());

            try {
                await _db.RemoveUserAsync(id).ConfigureAwait(false);
            }
            catch {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }

            return Ok();
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="oldPassword">Current user password.</param>
        /// <param name="password">New user password.</param>
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that password changing is success.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find or password is invalid.</response>
        [Authorize]
        [HttpPost("~/change/{oldPassword}/{password}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> ChangePasswordAsync(string oldPassword, string password)
        {
            try {
                var id = Utils.ExtractUserIdentifierFromToken(HttpContext);
                if (id == null)
                {
                    Log.Warn("Can't extract user idetntifier from JWT.");
                    return Unauthorized();
                }

                var castId = (int)id;
                Log.Info($"User with identifier \'{castId}\' is trying to change treir password.");
                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == castId)
                    .ConfigureAwait(false);

                if (user == null || !_passwordService.VerifyPassword(user.Hash, oldPassword, user.Salt))
                    return Unauthorized();

                var (salt, hash) = _passwordService.Hash(password);
                await _db.ChangePasswordAsync(castId, salt, hash).ConfigureAwait(false);

                var tokenDto = ProcessToken(user, ExpiredTokenTimeSpan);
                return Ok(tokenDto);
            }
            catch (BadPasswordException) {
                return Unauthorized();
            }
            catch (EntityNotFoundException) {
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
        /// <response code="200">Returns value is indicated that user signed in.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if request pair of login-password is incorrect.</response>
        [HttpPost("~/signin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> SignInUserAsync([FromBody] SignInDto dto)
        {
            Log.Info($"User with login \'{dto.UserLogin}\' is trying to sign in.");

            try {
                if (!_validator.Validate(dto))
                {
                    Log.Warn("User sent invalid DTO.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.UserLogin == dto.UserLogin)
                    .ConfigureAwait(false);

                if (user == null) return Unauthorized();
                if (user.PasswordExpired < DateTime.Today)
                {
                    Log.Warn("User's password expired. User will be removed!");
                    await _db.RemoveUserAsync(user.Id).ConfigureAwait(false);
                    return Unauthorized();
                }
                
                if (!_passwordService.VerifyPassword(user.Hash, dto.Password, user.Salt)) return Unauthorized();
                var token = await AuthenticateAsync(user).ConfigureAwait(false);
                var responseData = new UserInfoDto
                    {
                        UserId = user.Id,
                        IsAdmin = user.Role == UserRole.Administrator,
                        ShiftIsOpened = await _db.GetWorkingShiftStateAsync(user.Id).ConfigureAwait(false),
                        UserDisplayedName = $"{user.Name} {user.Surname}",
                        AssessToken = token
                    };

                return Ok(responseData);
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
        /// <returns><see cref="Task"/> for response.</returns>
        /// <response code="200">Returns value is indicated that user signed out.</response>
        /// <response code="400">Returns if requested data is invalid.</response>
        /// <response code="401">Returns if user didn't find.</response>
        [HttpPost("~/signout/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Dictionary<string, string>))]
        public async Task<IActionResult> SignOutUserAsync(int id)
        {
            Log.Info($"User with identifier \'{id}\' is trying to sign out.");

            try {
                if (id < 1) {
                    Log.Warn("User's identifier is incorrect.");
                    return BadRequest(_bodyBuilder.InvalidDtoBuild());
                }

                var user = await _db.Users
                    .SingleOrDefaultAsync(u => u.Id == id)
                    .ConfigureAwait(false);

                if (user == null)
                    return Unauthorized();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch {
                return BadRequest(_bodyBuilder.UnknownBuild());
            }
        }

        /// <summary>
        /// Processing token.
        /// </summary>
        /// <param name="user"><see cref="User"/>.</param>
        /// <param name="minutes">Expired time.</param>
        /// <returns>Access token.</returns>
        [NotNull]
        private string ProcessToken(User user, int minutes)
        {
            var credentials = new SigningCredentials(_keyExtractor.Extract(), SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiredTime = DateTime.Now.AddMinutes(minutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = CreateClaimsIdentity(user),
                Expires = expiredTime,
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

        /// <summary>
        /// Authenticate user with cookie.
        /// </summary>
        /// <param name="user"><see cref="User"/>.</param>
        /// <returns><see cref="Task"/> with access token.</returns>
        private async Task<string> AuthenticateAsync(User user)
        {
            var claims = new List<Claim> { new (ClaimsIdentity.DefaultNameClaimType, user.UserLogin), new ("Id", user.Id.ToString()) };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            return ProcessToken(user, ExpiredTokenTimeSpan);
        }
    }
}
