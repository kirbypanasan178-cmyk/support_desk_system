using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportDeskSystem.Web.Data;
using SupportDeskSystem.Web.Enums;
using SupportDeskSystem.Web.Models;

namespace SupportDeskSystem.Web.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // Admin creates a user
        public async Task<User> CreateUserAsync(
            string fullName,
            string email,
            string password,
            UserRole role)
        {
            var user = new User
            {
                FullName = fullName,
                Email = email,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Update user information
        public async Task<bool> UpdateUserAsync(
            int id,
            string fullName,
            string email,
            UserRole role)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return false;

            user.FullName = fullName;
            user.Email = email;
            user.Role = role;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
        // Activate / deactivate user
        public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return false;

            user.IsActive = isActive;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}