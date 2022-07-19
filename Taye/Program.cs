using Taye.Data;
using Taye.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Debugging;
using Taye.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Taye.Areas.Identity;
using Quartz;
using Taye.Jobs;
using Taye.Extensions;

SelfLog.Enable(Console.Error);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json")
//    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
//    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

Log.Logger.Information("log component is ready");

builder.Services.AddAntDesign();
builder.Services.AddHttpClient();
builder.Services.AddScoped<FileHelper>();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddDbContext<TayeContext>(options =>
options.UseNpgsql(config.GetSection("Connectionstrings").GetValue<string>("Connectionstring")));
builder.Services.AddDefaultIdentity<IdentityUser<int>>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<TayeContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = false;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

//builder.Services.Configure<PasswordHasherOptions>(options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(20);
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddTransient<SeedData>();
//builder.Services.Configure<SeedData>(sd => sd.SeedAdminUser());

//quartz
builder.Services.AddQuartz(q =>
{
    //var jk = new JobKey("Test");
    //q.UseMicrosoftDependencyInjectionJobFactory();
    //q.AddJob<TestJob>(o => o.WithIdentity(jk));
    //// Create a trigger for the job
    //q.AddTrigger(opts => opts
    //    .ForJob(jk) // link to the HelloWorldJob
    //    .WithIdentity("jk-trigger") // give the trigger a unique name
    //    .WithCronSchedule("0/5 * * * * ?")); // run every 5 seconds
});
builder.Host.ConfigureServices((hostContext, services) =>
services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    //q.AddJobAndTrigger<TestJob>(hostContext.Configuration);
    q.AddDelayOnce<EnsureAdministratorJob>();
}));
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication(); ;
app.UseRouting();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Log.Logger.Information("app prepared to start");
app.Run();
Log.Logger.Information("app end");