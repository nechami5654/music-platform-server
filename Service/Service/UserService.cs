using AutoMapper;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class UserService : IService<UserDto>
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;

        public UserService(IRepository<User> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> AddAsync(UserDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var userEntity = _mapper.Map<User>(item);
                var addedUser = await _repository.AddAsync(userEntity);
                return _mapper.Map<UserDto>(addedUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user.", ex);
            }
        }

        public async Task<UserDto> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var user = await _repository.GetAsync(id);
                if (user == null)
                    throw new Exception("User not found.");

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user.", ex);
            }
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            try
            {
                var users = await _repository.GetAllAsync();
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users.", ex);
            }
        }

        public async Task<UserDto> UpdateAsync(int id, UserDto item)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var userEntity = _mapper.Map<User>(item);
                var updatedUser = await _repository.UpdateAsync(id, userEntity);
                return _mapper.Map<UserDto>(updatedUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user.", ex);
            }
        }
    }
}
