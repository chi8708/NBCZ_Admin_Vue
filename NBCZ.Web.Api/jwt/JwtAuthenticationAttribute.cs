using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using WebApi.Jwt;
using WebApi.Jwt.Filters;

namespace NBCZ.Web.Api
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple {get;set;}

        public JwtAuthenticationAttribute(){
           AllowMultiple=false;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }



        private static bool ValidateToken(string token,out IPrincipal simplePrinciple, out string username)
        {
            username = null;

            simplePrinciple = JwtManager.GetPrincipal(token);
            var identity = (simplePrinciple==null?null:simplePrinciple.Identity) as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim==null ?null: usernameClaim.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            IPrincipal simplePrinciple = null;
            var username = "";
            if (ValidateToken(token,out simplePrinciple,out username))
            {
                // based on username to get more information from database in order to build local identity
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, username)
                //    // Add more claims if needed: Roles, ...
                //};

                //var identity = new ClaimsIdentity(claims, "Jwt");
                //IPrincipal user = new ClaimsPrincipal(identity);

                //return Task.FromResult(user);

                //cts 以上修改
                return Task.FromResult(simplePrinciple);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}
