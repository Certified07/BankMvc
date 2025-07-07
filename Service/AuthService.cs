using System;
using System.Text;
using System.Threading.Tasks;
using BankMvc.Models.Entity;
using BankMvc.Contract.Repository;
using System.Security.Cryptography;
using BankMvc.Contract.Service;
using BankMvc.DTO.ViewModels;
using Microsoft.Extensions.Logging;

namespace BankMvc.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger = null)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger;
        }

        public AuthResult Register(RegisterViewModel model)
        {
            return RegisterAsync(model).GetAwaiter().GetResult();
        }

        public async Task<AuthResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Validate input
                if (model == null)
                    return AuthResult.Failure("Registration data is required.");

                if (string.IsNullOrWhiteSpace(model.Username) ||
                    string.IsNullOrWhiteSpace(model.Password) ||
                    string.IsNullOrWhiteSpace(model.Email))
                {
                    return AuthResult.Failure("Username, password, and email are required.");
                }

                // Validate password strength
                if (!IsPasswordValid(model.Password))
                {
                    return AuthResult.Failure("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                }

                // Validate email format
                if (!IsEmailValid(model.Email))
                {
                    return AuthResult.Failure("Please provide a valid email address.");
                }

                // Check if username already exists using async method
                if (await UsernameExistsAsync(model.Username))
                {
                    return AuthResult.Failure("Username already exists.");
                }

                // Check if email already exists using async method
                if (await EmailExistsAsync(model.Email))
                {
                    return AuthResult.Failure("Email address is already registered.");
                }

                // Create user using the new async method
                User user = new User
                {
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                await CreateUserAsync(user, model.Password);

                _logger?.LogInformation("User registered successfully: {Username}", model.Username);
                return AuthResult.Success("User registered successfully.", user);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred during registration for username: {Username}", model?.Username);
                return AuthResult.Failure("An error occurred during registration. Please try again.");
            }
        }

        public AuthResult Login(LoginViewModel model)
        {
            return LoginAsync(model).GetAwaiter().GetResult();
        }

        public async Task<AuthResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                // Validate input
                if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return AuthResult.Failure("Username and password are required.");
                }

                // Get user from database
                User user = _userRepository.GetByUsername(model.Username);
                if (user == null)
                {
                    _logger?.LogWarning("Login attempt with non-existent username: {Username}", model.Username);
                    return AuthResult.Failure("Invalid username or password.");
                }

                // Verify password
                string passwordHash = HashPassword(model.Password, user.Salt);
                if (passwordHash != user.PasswordHash)
                {
                    _logger?.LogWarning("Failed login attempt for username: {Username}", model.Username);
                    return AuthResult.Failure("Invalid username or password.");
                }

                // Update last login time using the new async method
                await UpdateLastLoginAsync(user.UserId);

                _logger?.LogInformation("Successful login for username: {Username}", model.Username);
                return AuthResult.Success("Login successful.", user);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred during login for username: {Username}", model?.Username);
                return AuthResult.Failure("An error occurred during login. Please try again.");
            }
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return null;
                }

                // Get user from database (assuming you have an async method in repository)
                // If your repository doesn't have async methods, you can wrap the synchronous call
                User user = await Task.Run(() => _userRepository.GetByUsername(username));

                if (user == null)
                {
                    _logger?.LogWarning("Authentication attempt with non-existent username: {Username}", username);
                    return null;
                }

                // Verify password
                string passwordHash = HashPassword(password, user.Salt);
                if (passwordHash != user.PasswordHash)
                {
                    _logger?.LogWarning("Failed authentication attempt for username: {Username}", username);
                    return null;
                }

                // Update last activity time
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);

                _logger?.LogInformation("Successful authentication for username: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred during authentication for username: {Username}", username);
                return null;
            }
        }

        public bool IsUsernameAvailable(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return false;

                return _userRepository.GetByUsername(username) == null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking username availability for: {Username}", username);
                return false;
            }
        }

        public void Logout()
        {
            // This method can be extended to handle logout logic
            // such as clearing sessions, updating last activity, etc.
            // For now, it's a placeholder for future enhancements
            _logger?.LogInformation("User logout initiated");
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return false;

                // Assuming you have an async method in repository, otherwise wrap sync call
                var user = await Task.Run(() => _userRepository.GetByUsername(username));
                return user != null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking username existence for: {Username}", username);
                return false;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                // Assuming you have GetByEmail method in repository
                // If not available, you might need to implement it in the repository
                var user = await Task.Run(() => _userRepository.GetByEmail(email));
                return user != null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking email existence for: {Email}", email);
                return false;
            }
        }

        public async Task CreateUserAsync(User user, string password)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be null or empty", nameof(password));

                // Validate password strength
                //if (!IsPasswordValid(password))
                //{
                //    throw new ArgumentException("Password does not meet security requirements", nameof(password));
                //}

                // Generate salt and hash password
                string salt = GenerateSalt();
                string passwordHash = HashPassword(password, salt);

                // Set password hash and salt
                user.PasswordHash = passwordHash;
                user.Salt = salt;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                // Save to database
                bool isCreated = await Task.Run(() => _userRepository.Create(user));

                if (!isCreated)
                {
                    throw new InvalidOperationException("Failed to create user in database");
                }

                _logger?.LogInformation("User created successfully: {Username}", user.Username);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating user: {Username}", user?.Username);
                throw;
            }
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("Invalid user ID", nameof(userId));

                // Get user by ID
                var user = await Task.Run(() => _userRepository.GetById(userId));
                if (user == null)
                {
                    _logger?.LogWarning("Attempted to update last login for non-existent user ID: {UserId}", userId);
                    throw new InvalidOperationException($"User with ID {userId} not found");
                }

                // Update last login time
                user.UpdatedAt = DateTime.UtcNow;

                // If you have a specific LastLoginAt property, uncomment and use it
                // user.LastLoginAt = DateTime.UtcNow;

                bool isUpdated = await Task.Run(() => _userRepository.Update(user));

                if (!isUpdated)
                {
                    throw new InvalidOperationException("Failed to update user's last login time");
                }

                _logger?.LogInformation("Last login updated for user ID: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating last login for user ID: {UserId}", userId);
                throw;
            }
        }

        #region Private Helper Methods

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32]; // 256-bit salt for better security
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    // Supporting classes for MVC pattern
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public User User { get; set; }

        public static AuthResult Success(string message, User user = null)
        {
            return new AuthResult
            {
                IsSuccess = true,
                Message = message,
                User = user
            };
        }

        public static AuthResult Failure(string message)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = message,
                User = null
            };
        }
    }
}