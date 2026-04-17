using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;

namespace RPCMAS.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;
        public AuthRepository(AppDbContext dbContext) 
        { 
            _dbContext = dbContext;
        
        }

        public Task<UserModel> GetUserByLogin(string username, string password)
        {
            return _dbContext.Users.Include(n => n.UserRoles).ThenInclude(n => n.Role).FirstOrDefaultAsync(n => n.Username == username && n.Password == password);
        }
        public async Task RemoveRefreshTokenByUserID(int userID)
        {
            var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(n => n.UserID == userID);
            if (refreshToken != null)
            {
                _dbContext.RemoveRange(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshTokenModel);
            await _dbContext.SaveChangesAsync();
        }

        public Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken)
        {
            return _dbContext.RefreshTokens.Include(n => n.User).ThenInclude(n => n.UserRoles).ThenInclude(n => n.Role).FirstOrDefaultAsync(n => n.RefreshToken == refreshToken);
        }
    }
}
