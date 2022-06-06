using Api.Exceptions;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class Repository : IRepository
    {
        private readonly TokenContext _token;
        private readonly TesteDbContext _context;
        private bool disposedValue;

        public Repository(TokenContext token, TesteDbContext context)
        {
            _token = token;
            _context = context;
        }

        public async Task<Token> InvalidateTokenAsync(string token)
        {
            var invalidate = new Token()
            {
                IsValid = false,
                Value = token
            };
            var resp = _token.Tokens.Add(invalidate);
            await _token.SaveChangesAsync();
            return resp.Entity;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var tok = await _token.Tokens.FirstOrDefaultAsync(x => x.Value == token);
            if (tok == null)
            {
                return true;
            }
            return tok.IsValid;
        }

        public async Task<Token> RevalidateTokenAsync(string token)
        {
            var tok = await _token.Tokens.FirstOrDefaultAsync(x => x.Value == token);
            if (tok == null)
            {
                throw new InvalidTokenException();
            }
            var validate = new Token()
            {
                Id = tok.Id,
                IsValid = true,
                Value = token
            };
            var resp = _token.Tokens.Update(validate);
            await _token.SaveChangesAsync();
            return resp.Entity;
        }
        public async Task<User> InvalidateUserAsync(User user)
        {
            var resp = _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return resp.Entity;
        }

        public async Task<bool> IsUserValidAsync(string name)
        {
            var usr = await _context.Users.FirstOrDefaultAsync(x => x.Username == name);
            if (usr == null)
            {
                return true;
            }
            return usr.IsValid;
        }

        public async Task<User> RevalidateUserAsync(User user)
        {
            var usr = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
            if (usr == null)
            {
                throw new UserNotFoundException();
            }
            var validate = new User()
            {
                Id = usr.Id,
                IsValid = true,
                Username = user.Username,
                Email = user.Email,
                RequestLimit = user.RequestLimit
            };
            var resp = _context.Users.Update(validate);
            await _context.SaveChangesAsync();
            return resp.Entity;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TokenRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task InvalidateBoth(string token, User user)
        {
            var invalidate = new Token()
            {
                IsValid = false,
                Value = token
            };
            _token.Tokens.Add(invalidate);
            await _token.SaveChangesAsync();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
