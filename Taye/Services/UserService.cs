using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Taye.Models;

namespace Taye.Services
{
    public interface IUserService
    {
        Task<IdentityUser> GetCurrentUserAsync();
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IdentityUser> GetCurrentUserAsync()
        {
            return new IdentityUser();
        }
    }
}