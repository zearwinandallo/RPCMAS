using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;

namespace RPCMAS.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserModel> GetUserByLogin(string username, string password);
        Task RemoveRefreshTokenByUserID(int userID);
        Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel);
        Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken);
    }
}
