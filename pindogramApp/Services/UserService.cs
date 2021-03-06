﻿using System;
using System.Collections.Generic;
using pindogramApp.Entities;
using pindogramApp.Helpers;
using System.Linq;
using AutoMapper;
using pindogramApp.Services.Interfaces;

namespace pindogramApp.Services
{
    public class UserService : IUserService
    {
        private readonly PindogramDataContext _context;
        private readonly IMapper _mapper;

        public UserService(PindogramDataContext context)
        {
            _context = context;
        }

        public void AddUserToAdminGroup(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) throw new AppException($"Nie ma usera o id równym {userId}");

            user.GroupId = 1;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void AddUserToUserGroup(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) throw new AppException($"Nie ma usera o id równym {userId}");

            user.GroupId = 2;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            user.Group = _context.Groups.FirstOrDefault(x => x.Id == user.GroupId);

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = new List<User>();
            var groups = _context.Groups;
            foreach (var user in _context.Users)
            {
                if (user.Group == null)
                    user.Group = groups.FirstOrDefault(x => x.Id == user.GroupId);

                users.Add(user);
            }

            return users;
        }

        public IEnumerable<User> GetAllUnactivatedUsers()
        {
            return _context.Users.Where(x => !x.IsActive);
        }

        public User ActiveUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.IsActive = true;
                _context.Users.Update(user);
                _context.SaveChanges();
                return user;
            }
            throw new AppException($"Nie ma usera o id równym {id}");
        }

        public User DeactiveUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.IsActive = false;
                _context.Users.Update(user);
                _context.SaveChanges();
                return user;
            }
            throw new AppException($"Nie ma usera o id równym {id}");
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            var groups = _context.Groups;
            if(user != null)
            {
                if (user.Group == null)
                    user.Group = groups.FirstOrDefault(x => x.Id == user.GroupId);
                return user;
            }
            throw new AppException($"Nie ma usera o id równym {id}");
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Hasło jest wymagane");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException($"Użytkownik {user.Username} jest już zajęty");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.Group = _context.Groups.LastOrDefault();
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException($"Użytkownik {user.Username} jest już zajęty");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("Hasło nie może być puste");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Hasło nie może być puste.");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("Hasło nie może być puste.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Hasło nie może być puste");
            if (storedHash.Length != 64) throw new ArgumentException("Nie poprawna długość hasła.");
            if (storedSalt.Length != 128) throw new ArgumentException("Nie poprawna długość hasła.");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
