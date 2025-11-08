using AutoMapper;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Configuration;
using Prueba.Application.Dtos;
using Prueba.Application.Interfaces;
using Prueba.Domain.Entities;
using Prueba.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Prueba.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IBlobStorageService _blobStorageService;
        private readonly string _blobBaseUrl;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IJwtProvider jwtProvider,
                           IPasswordHasher passwordHasher, IBlobStorageService blobStorageService,
                           IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _blobStorageService = blobStorageService;
            _blobBaseUrl = configuration["BlobStorage:BaseUrl"]; 
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null) return null; 

            if (!_passwordHasher.Verify(user.PasswordHash, password))
            {
                return null; 
            }

            return _jwtProvider.GenerateToken(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            var userDto = _mapper.Map<UserDto>(user);

            if (!string.IsNullOrEmpty(userDto.ProfilePictureUrl))
            {
                userDto.ProfilePictureUrl = $"{_blobBaseUrl}/{userDto.ProfilePictureUrl}";
            }

            return userDto;
        }

        public async Task<UserDto> RegisterAsync(string email, string password, string firstName, string lastName, IFormFile profilePicture)
        {
            if (await _unitOfWork.Users.GetByEmailAsync(email) != null)
            {
                throw new ValidationException("El email ya está registrado.");
            }


            var user = new User
            {
                Email = email,
                PasswordHash = _passwordHasher.Hash(password),
                FirstName = firstName,
                LastName = lastName
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync(); 
            if (profilePicture != null)
            {
                string path = $"users/{user.Id}/profile{Path.GetExtension(profilePicture.FileName)}";
                await _blobStorageService.UploadAsync(profilePicture.OpenReadStream(), path, profilePicture.ContentType);
                user.ProfilePicturePath = path; // Guardamos la RUTA

                _unitOfWork.Users.Update(user); 
                await _unitOfWork.CompleteAsync();
            }

            return await GetUserByEmailAsync(email);
        }


        public async Task<UserDto> GetUserByIdAsync(int id)
        {
    
            var user = await _unitOfWork.Users.GetByIdAsync(id); 


            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            
            var userDto = _mapper.Map<UserDto>(user);

            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                userDto.ProfilePictureUrl = _blobStorageService.GetFileUrl(user.ProfilePicturePath);
            }

            return userDto;
        }
    }
}