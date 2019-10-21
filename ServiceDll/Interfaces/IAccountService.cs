using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    interface IAccountService
    {
        UserModel Login(AccountModel model);
        Task<UserModel> LoginAsync(AccountModel model);
        List<string> Registration(UserModel model);
        Task<List<string>> RegistrationAsync(UserModel model);
    }
}
