using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NBCZ.Model;
using Microsoft.IdentityModel.Logging;

namespace WebApi.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
       //private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw=";

        private const string Secret = "NBCZWEBAPI1234567890";
       
        public static Tuple<string,DateTime?> GenerateToken(Pub_User user, int expireMinutes = 30)
        {
            //var symmetricKey = Convert.FromBase64String(Secret);
            byte[] symmetricKey = System.Text.Encoding.UTF8.GetBytes(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim("Id",user.Id.ToString()),
                    new Claim("UserCode",user.UserCode),
                   // new Claim(ClaimTypes.UserData,functionsStr),
                    new Claim("Tel",user.Tel),
                    new Claim("DeptCode",user.DeptCode)
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer="NBCZ.Web.Api",

                Subject = new ClaimsIdentity(claims),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

           // IDX10603: Decryption failed. Keys tried: '[PII is hidden]'.
            //解决方案
            //Secret key length should not be less than 128 bits
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return Tuple.Create(token,tokenDescriptor.Expires);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                //var symmetricKey = Convert.FromBase64String(Secret);
                byte[] symmetricKey = System.Text.Encoding.UTF8.GetBytes(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                   RequireExpirationTime = true,
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}