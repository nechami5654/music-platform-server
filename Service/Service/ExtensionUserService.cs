using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Service.Service
{
    public class ExtensionUserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public ExtensionUserService(IUserRepository repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }

        public string Generate(UserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim("username", user.Name), 
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("role", "user"),
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMonths(1),
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating JWT token.", ex);
            }
        }

        public async Task<UserDto> AuthenticateAsync(string userName, string pass)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name is required.", nameof(userName));
            if (string.IsNullOrWhiteSpace(pass))
                throw new ArgumentException("Password is required.", nameof(pass));

            try
            {
                var userEntity = await _repository.GetByNameAndPasswordAsync(userName, pass);
                if (userEntity == null)
                    throw new Exception("Authentication failed. User not found.");

                return _mapper.Map<UserDto>(userEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during authentication.", ex);
            }
        }

        public async Task<List<UserDto>> GetUsersByIdsAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("Invalid user IDs list.", nameof(ids));

            try
            {
                var users = await _repository.GetUsersByIdsAsync(ids);
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users by IDs.", ex);
            }
        }

        public async Task<bool> ValidUserPasswordAsync(int id, string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                bool response = await _repository.ValidUserPasswordAsync(id, password);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating user credentials.", ex);
            }
        }
    }
}
