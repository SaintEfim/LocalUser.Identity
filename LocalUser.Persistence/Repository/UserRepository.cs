using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Users.Domain.Entity;
using Users.Domain.Interface;

namespace Users.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string secretKey;

        public UserRepository(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.LocalUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var user = _context.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequest.UserName.ToLower() && u.Password == loginRequest.Password);

            if (user == null)
            {
                return new LoginResponse()
                {
                    Token = "",
                    User = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (token is JwtSecurityToken jwtToken)
            {
                var tokenString = tokenHandler.WriteToken(token);
                Console.WriteLine("Generated token: " + tokenString); // Печать токена для проверки
            }

            LoginResponse loginResponse = new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };
            return loginResponse;
        }


        public async Task<LocalUser> Register(RegistrationRequest registrationRequest)
        {
            LocalUser user = _mapper.Map<LocalUser>(registrationRequest);

            _context.LocalUsers.Add(user);
            await _context.SaveChangesAsync();
            user.Password = "";
            return user;
        }
    }
}
