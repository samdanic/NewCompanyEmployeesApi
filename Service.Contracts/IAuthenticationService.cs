using Microsoft.AspNetCore.Identity;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UsersForRegistrationDto usersForRegistration);
        Task<bool>ValidateUser(UsersForAuthenticationDto usersForAuthentication);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto token);
        
    }
}
