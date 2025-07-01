using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Entity;

namespace BankMvc.Contract.Repository
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByUsername(string username);
        bool Create(User user);
        bool Update(User user);
        bool Delete(int userId);
        List<User> GetAll();
        User GetByEmail(string email);
    }
}