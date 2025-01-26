using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Prog.Services
{
    
    public class AuthOptions
    {
        public const string ISSUER = "HeapSort";
        public const string AUDIENCE = "HeapSortAudience";
        public static SymmetricSecurityKey GetKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HeapSortHeapSortHeapSortHeapSortHeapSortHeapSortHeapSortHeapSort"));
        }
    }
}