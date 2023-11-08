using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain.Entity;

namespace Users.Domain.Interface
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponse> Login(LoginRequest loginRequesrt);
        Task<LocalUser> Register(RegistrationRequest registrationRequest);
    }
}
