using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Dapper;

namespace Infrastructure.Repositories
{
     public class UserDapperRepo : IUserDapperRepo
    {
        private readonly DapperContext _context;
        public UserDapperRepo(DapperContext context)
        {
            _context = context;
        }


        public async Task<int> CreateUserAsync(User user)
        {
            var query = @"INSERT INTO USER (Id, UserName, Email, PhoneNumber)  
                         VALUES (@Id, @UserName, @Email, @FullName, @PhoneNumber)";
            using(var connection = _context.CreateConnection())
            {
                var data = await connection.ExecuteAsync(query, user);
                return data;
            }
        }

        public async Task<int> DeleteUserAsync(string id)
        {
            var query = @"DELETE from Users Where Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.ExecuteAsync(query, new { Id = id });
                return data;
            }
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            var query = @"
                          Select u.Id, u.UserName , u.Email, u.PhoneNumber , r.Name
                          From Users u
                          Left Join UserRoles ur ON u.Id = ur.UserId
                          Left Join Roles r ON ur.RoleId = r.Id
                           ";

            using (var connection = _context.CreateConnection())
            {
                var userDict = new Dictionary<string, User>();

                var data = await connection.QueryAsync<User, Role, User>(
                    query,
                    (user, role) =>
                    {
                        if (!userDict.TryGetValue(user.Id, out var currentUser))
                        {
                            currentUser = user;
                            currentUser.Roles = new List<string>();
                            userDict.Add(currentUser.Id, currentUser);
                        }

                        if (role != null && !string.IsNullOrEmpty(role.Name))
                        {
                            if (currentUser.Roles == null)
                                currentUser.Roles = new List<string>();
                            currentUser.Roles.Add(role.Name);
                        }
                        return currentUser;
                    },
                    splitOn: "Name"
                );

                return userDict.Values;
            }
        }



        public async Task<User> GetUserByIdAsync(string id)
        {
            var query = @"Select Id, UserName , Email, PhoneNumber from Users where Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
                return user;
            }
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            var query = @"UPDATE Users
                          SET UserName = @UserName, Email = @Email, PhoneNumber = @PhoneNumber
                          Where Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var data = await connection.ExecuteAsync(query, user);
                return data;
            }
        }



    }
}
