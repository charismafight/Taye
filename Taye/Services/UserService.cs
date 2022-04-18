using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Taye.Models;

namespace Taye.Services
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync();
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return new User();
        }
    }
}