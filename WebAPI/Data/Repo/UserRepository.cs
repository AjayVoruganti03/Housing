using System.Threading.Tasks;
using WebAPI.Interfaces;
using WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User?> Authenticate(string username, string passwordText)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null || user.PasswordSalt==null) return null;

            if (!MatchPasswordHash(passwordText, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        private bool MatchPasswordHash(string passwordText, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public async Task Register(string username, string password)
        {
            byte[] passwordHash, passwordSalt;

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.Users.AddAsync(user);
        }

        public async Task<bool> UserAlreadyExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }
    }
}
