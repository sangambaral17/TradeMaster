using System.Security.Cryptography;
using System.Text;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for user authentication and session management.
    /// </summary>
    public class AuthenticationService
    {
        private readonly IRepository<User> _userRepository;
        private User? _currentUser;

        public AuthenticationService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Gets the currently logged in user, or null if no user is logged in.
        /// </summary>
        public User? CurrentUser => _currentUser;

        /// <summary>
        /// Indicates whether a user is currently logged in.
        /// </summary>
        public bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// Event raised when login state changes.
        /// </summary>
        public event EventHandler? LoginStateChanged;

        /// <summary>
        /// Attempts to authenticate a user with the given credentials.
        /// </summary>
        public async Task<bool> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var users = await _userRepository.FindAsync(u => 
                u.Username.ToLower() == username.ToLower() && u.IsActive);
            
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                return false;
            }

            // Update last login date
            user.LastLoginDate = DateTime.Now;
            await _userRepository.UpdateAsync(user);

            _currentUser = user;
            LoginStateChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
            LoginStateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks if the current user has the specified role.
        /// </summary>
        public bool IsInRole(string role)
        {
            if (_currentUser == null) return false;
            return _currentUser.Role == role || _currentUser.Role == UserRoles.Admin;
        }

        /// <summary>
        /// Checks if the current user is an administrator.
        /// </summary>
        public bool IsAdmin => _currentUser?.Role == UserRoles.Admin;

        /// <summary>
        /// Checks if the current user is a manager or higher.
        /// </summary>
        public bool IsManagerOrHigher => 
            _currentUser?.Role == UserRoles.Manager || 
            _currentUser?.Role == UserRoles.Admin;

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        public async Task<User> CreateUserAsync(string username, string password, string role, string? fullName = null, string? email = null)
        {
            // Check if username already exists
            var existingUsers = await _userRepository.FindAsync(u => u.Username.ToLower() == username.ToLower());
            if (existingUsers.Any())
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = role,
                FullName = fullName,
                Email = email,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        public async Task DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Creates a password hash using SHA256.
        /// Note: For production use, consider using BCrypt or Argon2.
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "TradeMasterSalt2025"));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        private static bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }
    }
}
