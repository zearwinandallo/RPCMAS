using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPCMAS.Core.Entities;

namespace RPCMAS.Core.Interfaces
{
    public interface IAuthService
    {
        Task<UserModel> GetUserByLogin(string username, string password);
        Task AddRefreshTokenModel(RefreshTokenModel refreshTokenModel);
        Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken);
    }
}
