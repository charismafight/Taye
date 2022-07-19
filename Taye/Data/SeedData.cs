using Microsoft.AspNetCore.Identity;
using Taye.Repositories;

namespace Taye.Data
{
    public class SeedData
    {
        private readonly TayeContext _context;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IUserStore<IdentityUser<int>> _userStore;
        private readonly IUserEmailStore<IdentityUser<int>> _emailStore;
        private readonly IRoleStore<IdentityRole<int>> _roleStore;
        private readonly ILogger<SeedData> _logger;

        public SeedData(TayeContext context,
                        UserManager<IdentityUser<int>> userManager,
            IUserStore<IdentityUser<int>> userStore,
            SignInManager<IdentityUser<int>> signInManager,
            IRoleStore<IdentityRole<int>> roleStore,
            ILogger<SeedData> logger)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _roleStore = roleStore;
            _logger = logger;
            _emailStore = GetEmailStore();
        }

        private IUserEmailStore<IdentityUser<int>> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser<int>>)_userStore;
        }

        public async void SeedAdminUser()
        {
            if (!_context.Users.Any(p => p.Id == 1))
            {
                //默认用户数据
                var user = new IdentityUser<int>()
                {
                    Id = 1,
                    UserName = "admin",
                    Email = "charismafight@hotmail.com",
                    NormalizedUserName = "admin",
                    PhoneNumber = "17771819183",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "123");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"用户{user.UserName}创建成功");
                }
                else
                {
                    _logger.LogInformation($"用户{user.UserName}创建失败");
                }
            }

            if (!_context.Roles.Any(p => p.Id == 1 || p.Id == 2))
            {
                //构造管理员用户完毕，开始处理用户角色
                //系统角色
                var adminRole = new IdentityRole<int>
                {
                    Id = 1,
                    Name = Common.Security.Roles.Admin.ToString(),
                    NormalizedName = Common.Security.Roles.Admin.ToString().ToUpper()
                };
                var userRole = new IdentityRole<int>
                {
                    Id = 2,
                    Name = Common.Security.Roles.User.ToString(),
                    NormalizedName = Common.Security.Roles.User.ToString().ToUpper()
                };
                await _roleStore.CreateAsync(userRole, CancellationToken.None);
                await _roleStore.CreateAsync(adminRole, CancellationToken.None);
                _context.SaveChanges();
                _logger.LogInformation($"基础角色user、admin创建成功");
                await _userManager.AddToRoleAsync(_context.Users.Single(p => p.Id == 1), adminRole.Name);
                _logger.LogInformation($"admin已加入到管理员角色");
                _context.SaveChanges();
            }
        }
    }
}
