using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;

using VipcoTransport.Classes;
using VipcoTransport.Models;
using VipcoTransport.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VipcoTransport.Controllers
{
    [Route("api/[controller]")]
    public class JwtController : Controller
    {
        #region Private Members

        private readonly JwtIssuerOptions jwtOptions;
        private readonly ILogger logger;
        private readonly JsonSerializerSettings serializerSettings;
        private IEmployeeRepository repository;

        #endregion Private Members

        public JwtController(IOptions<JwtIssuerOptions> _jwtOptions, ILoggerFactory _loggerFactory, IEmployeeRepository repo)
        {
            this.repository = repo;
            this.jwtOptions = _jwtOptions.Value;
            ThrowIfInvalidOptions(this.jwtOptions);

            this.logger = _loggerFactory.CreateLogger<JwtController>();

            this.serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromForm] TblUser applicationUser)
        {
            try
            {
                var identity = await GetClaimsIdentity(applicationUser);
                if (identity == null)
                {
                    this.logger.LogInformation($"Invalid username ({applicationUser.Username}) or password ({applicationUser.Password})");
                    return BadRequest("Invalid credentials");
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, await this.jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(this.jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                    identity.FindFirst("Administrator"),
                    // claim for muilt aud
                    new Claim("aud", this.jwtOptions.Audience),
                    new Claim("aud", this.jwtOptions.Audience2),
                    new Claim("aud", this.jwtOptions.Audience3)

                };

                // Create the JWT security token and encode it.
                var jwt = new JwtSecurityToken(
                    issuer: this.jwtOptions.Issuer,
                    // audience: this.jwtOptions.Audience, //cancel this audience it already add in Claim
                    claims: claims,
                    notBefore: this.jwtOptions.NotBefore,
                    expires: this.jwtOptions.Expiration,
                    signingCredentials: this.jwtOptions.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // Serialize and return the response
                var response = new
                {
                    token = encodedJwt,
                    expires_in = DateTime.Now.AddMinutes(this.jwtOptions.ValidFor.Minutes)
                    //expires_in = DateTime.Now.AddSeconds(30)
                };

                var json = JsonConvert.SerializeObject(response, this.serializerSettings);
                return new OkObjectResult(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
                return new StatusCodeResult(500);
            }
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
              => (long)Math.Round((date.ToUniversalTime() -
                                   new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                                  .TotalSeconds);
        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        [HttpGet("UserLoginExpochDate")]
        [AllowAnonymous]
        public IActionResult GetUserLoginExpochDate(string UserLoginExpoch)
        {
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: this.jwtOptions.Issuer,
                // audience: this.jwtOptions.Audience, //cancel this audience it already add in Claim
                claims: null,
                notBefore: this.jwtOptions.NotBefore,
                expires: this.jwtOptions.Expiration,
                signingCredentials: this.jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                token = JwtIssuerOptions2.Issuer,
                expires_in = DateTime.Now.AddMinutes(this.jwtOptions.ValidFor.Minutes)
                //expires_in = DateTime.Now.AddSeconds(30)
            };
            var json = JsonConvert.SerializeObject(response, this.serializerSettings);
            return new OkObjectResult(json);
        }
        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private Task<ClaimsIdentity> GetClaimsIdentity(TblUser user)
        {
            if (this.repository.CheckUserProfile(user.Username, user.Password))
            {
                return Task.FromResult(
                    new ClaimsIdentity(
                        new GenericIdentity(user.Username, "Token"),
                        new[]
                        {
                            new Claim("Administrator", "admin")
                        })
                  );
            }

            //if (user.UserName == "user" &&
            //    user.Password == "qwer1234")
            //{
            //    return Task.FromResult(new ClaimsIdentity(
            //      new GenericIdentity(user.UserName, "Token"),
            //      new Claim[] { }));
            //}

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}