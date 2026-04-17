using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Infrastructure.Repositories;

namespace RPCMAS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository) 
        { 
            _authRepository = authRepository;
        }

        public Task<UserModel> GetUserByLogin(string username, string password)
        {
            return _authRepository.GetUserByLogin(username, password);
        }
        public async Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel)
        {
            await _authRepository.RemoveRefreshTokenByUserID(refreshTokenModel.UserID);
            await _authRepository.AddRefreshTokenModel(refreshTokenModel);
        }
        public Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken)
        {
            return _authRepository.GetRefreshTokenModel(refreshToken);
        }
    }
}
