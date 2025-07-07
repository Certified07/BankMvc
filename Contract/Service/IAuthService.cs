//using BankMvc.Models.Entity;
//using BankMvc.DTO;
//using BankMvc.Services;
//using BankMvc.DTO.ViewModels;

//namespace BankMvc.Contract.Service
//{
//    //public interface IAuthService
//    //{
//    //    AuthResult Register(RegisterViewModel model);
//    //    AuthResult Login(LoginViewModel model);
//    //    bool IsUsernameAvailable(string username);
//    //    void Logout();
//    //    Task<User> AuthenticateAsync(string username, string password);
//    //    Task<bool> UsernameExistsAsync(string username);
//    //    Task<bool> EmailExistsAsync(string email);
//    //    Task CreateUserAsync(User user, string password);
//    //    Task UpdateLastLoginAsync(int userId);
//    //    //bool ChangePassword(int userId, string currentPassword, string newPassword);  
//    //    //bool ValidateCredentials(string username, string password);  
//    //}

//}
using BankMvc.Models.Entity;
using BankMvc.DTO;
using BankMvc.DTO.ViewModels;
using BankMvc.Service;

namespace BankMvc.Contract.Service
{
    public interface IAuthService
    {
        // Existing methods - keeping sync versions for backward compatibility
        AuthResult Register(RegisterViewModel model);
        AuthResult Login(LoginViewModel model);
        bool IsUsernameAvailable(string username);
        void Logout();
        Task<User> AuthenticateAsync(string username, string password);

        // New async versions of existing methods
        Task<AuthResult> RegisterAsync(RegisterViewModel model);
        Task<AuthResult> LoginAsync(LoginViewModel model);

        // New async methods
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task CreateUserAsync(User user, string password);
        Task UpdateLastLoginAsync(int userId);

        // Commented out methods that could be implemented later
        //bool ChangePassword(int userId, string currentPassword, string newPassword);
        //bool ValidateCredentials(string username, string password);
    }
}