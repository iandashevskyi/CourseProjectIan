using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Prog.Services
{
    
    public class AuthOptions
    {
        public const string ISSUER = "SieveOfSund";
        public const string AUDIENCE = "SieveOfSundAudience";
        public static SymmetricSecurityKey GetKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SieveOfSundAudienceSieveOfSundAudienceSieveOfSundAudienceSieveOfSundAudiencePassword"));
        }
    }
}