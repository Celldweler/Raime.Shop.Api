using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api
{
    public struct RaimeShopConstants
    {
        public struct Policies
        {
            // public const string Anon = nameof(Anon);
            // public const string User = IdentityServerConstants.LocalApi.PolicyName;
            public const string Mod = nameof(Mod);
            public const string Admin = nameof(Admin);
        }

        public struct IdentityResources
        {
            public const string RoleScope = "role";
        }

        public struct Claims
        {
            public const string Role = "role";
        }

        public struct Roles
        {
            public const string Mod = nameof(Mod);
            public const string Admin = nameof(Admin);
        }
    }
}
