using System.Data.SqlTypes;
using FinalExamDAIS.Services.Interfaces.Authentication;
using FinalExamDAIS.Services.DTOs.Authentication;
using FinalExamDAIS.Repository.Interfaces.User;
using FinalExamDAIS.Models;
using FinalExamDAIS.Services.Helpers;

namespace FinalExamDAIS.Services.Implementations.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Потребителското име и паролата са задължителни"
                    };
                }

                var hashedPassword = SecurityHelper.HashPassword(request.Password);
                var filter = new UserFilter
                {
                    Username = request.Username
                };

                var users = _userRepository.RetrieveCollectionAsync(filter);
                User? user = null;
                await foreach (var u in users)
                {
                    user = u;
                    break;
                }

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Потребител с това име не съществува"
                    };
                }

                if (user.PasswordHash != hashedPassword)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Невалидна парола"
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Акаунтът е деактивиран"
                    };
                }

                return new LoginResponse
                {
                    Success = true,
                    Message = "Успешно влизане",
                    UserInfo = new UserInfo
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Възникна грешка при влизане. Моля, опитайте отново."
                };
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var filter = new UserFilter
                {
                    Username = request.Username
                };

                var existingUsers = _userRepository.RetrieveCollectionAsync(filter);
                var exists = false;
                await foreach (var _ in existingUsers)
                {
                    exists = true;
                    break;
                }

                if (exists)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Потребител с това име вече съществува"
                    };
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = SecurityHelper.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userRepository.CreateAsync(user);
                return new RegisterResponse
                {
                    Success = result > 0,
                    Message = result > 0 ? "Регистрацията беше успешна" : "Регистрацията не беше успешна",
                    UserInfo = new UserInfo
                    {
                        UserId = result,
                        Username = user.Username,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Възникна грешка при регистрация. Моля, опитайте отново."
                };
            }
        }
    }
}
