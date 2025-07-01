using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankMvc.Models.Entity;
using BankMvc.Contract.Repository;
 // Assuming you have a Data folder with DbContext

namespace BankMvc.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BankApplicationDbContext _context;

        public UserRepository(BankApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User GetById(int userId)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                // Log exception here  
                throw new Exception($"Error retrieving user with ID {userId}", ex);
            }
        }

        public User GetByUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return null;

                return _context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error retrieving user with username {username}", ex);
            }
        }


        public bool Create(User user)
        {
            try
            {
                if (user == null)
                    return false;

                // Check if username already exists
                if (GetByUsername(user.Username) != null)
                    return false;

                _context.Users.Add(user);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception("Error creating user", ex);
            }
        }

        public bool Update(User user)
        {
            try
            {
                if (user == null)
                    return false;

                var existingUser = GetById(user.UserId);
                if (existingUser == null)
                    return false;

                // Check if username is being changed and if new username already exists
                if (existingUser.Username != user.Username)
                {
                    var userWithSameUsername = GetByUsername(user.Username);
                    if (userWithSameUsername != null && userWithSameUsername.UserId != user.UserId)
                        return false;
                }

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error updating user with ID {user?.UserId}", ex);
            }
        }

        public User GetByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return null;
                return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error retrieving user with email {email}", ex);
            }
        }
        public bool Delete(int userId)
        {
            try
            {
                var user = GetById(userId);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error deleting user with ID {userId}", ex);
            }
        }

        public List<User> GetAll()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception("Error retrieving all users", ex);
            }
        }
    }
}