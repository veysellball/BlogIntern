using AutoMapper;
using BlogIntern.Data;
using BlogIntern.Dtos;
using BlogIntern.Models;
using BlogIntern.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogIntern.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
                throw new Exception($"User with id {id} not found or inactive.");

            return user;
        }
        public async Task<User> AddNewUser(UserCreateDto dto)
        {
            var user = _mapper.Map<User>(dto);

            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existing != null)
                throw new Exception("Bu email zaten kayıtlı. Lutfen baska bir mail girin");

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.InsertDate = DateTime.Now;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        public async Task<List<User>> GetAllUsersOrderByDate()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.InsertDate)
                .ToListAsync();
        }

        public async Task<bool> DeleteUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReActivateUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserRoleSpDto>> GetUsersWithRoles_SP()
        {
            return await _context.UserWithRoleDtos
                .FromSqlRaw("EXECUTE dbo.GetUsersWithRoles")
                .ToListAsync();
        }

        public async Task<List<UserRoleSpDto>> GetUsersByRole_SP(string roleName)
        {
            var param = new SqlParameter("@RoleName", roleName);

            return await _context.UserWithRoleSp
                .FromSqlRaw("EXEC spGetUsersByRole_New @RoleName", param)
                .ToListAsync();
        }



    }
}
