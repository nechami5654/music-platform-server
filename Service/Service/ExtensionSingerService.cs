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
    public class ExtensionSingerService : ISingerService
    {
        private readonly IConfiguration _config;
        private readonly ISingerRepository _repository;
        private readonly IMapper _mapper;

        public ExtensionSingerService(ISingerRepository repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }

        public string Generate(SingerDto singer)
        {
            if (singer == null)
                throw new ArgumentNullException(nameof(singer));

            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim("username", singer.Name), 
                    new Claim("id", singer.Id.ToString()),
                    new Claim("email", singer.Email),
                    new Claim("role", "singer"),
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

        public async Task<SingerDto> AuthenticateAsync(string singerName, string pass)
        {
            if (string.IsNullOrWhiteSpace(singerName))
                throw new ArgumentException("Singer name is required.", nameof(singerName));
            if (string.IsNullOrWhiteSpace(pass))
                throw new ArgumentException("Password is required.", nameof(pass));

            try
            {
                var singerEntity = await _repository.GetByNameAndPasswordAsync(singerName, pass);
                if (singerEntity == null)
                    throw new Exception("Authentication failed. Singer not found.");

                return _mapper.Map<SingerDto>(singerEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during authentication.", ex);
            }
        }

        public async Task<bool> ValidSingerPasswordAsync(int id, string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid singer ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                bool response = await _repository.ValidSingerPasswordAsync(id, password);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating singer credentials.", ex);
            }
        }
    }
}
