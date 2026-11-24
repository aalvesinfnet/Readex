using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Readex.Infraestrutura;
using Readex.Models;
using Readex.Servicos;
using System.Threading.RateLimiting;
namespace Readex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            var caminhoBanco = Path.Combine(
                builder.Environment.ContentRootPath, "Dados", "readex.db");

            var conexao = new SqliteConnectionStringBuilder {
                DataSource = caminhoBanco
            }.ToString();

            builder.Services.AddDbContext<ReadexDbContext>(
                options => options.UseSqlite(conexao));
            builder.Services.AddScoped<IPasswordHasher<Leitor>, PasswordHasher<Leitor>>();
            builder.Services.AddScoped<CadastroLeitoresPersistido>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/sign-in";
                    options.Cookie.Name = "Readex.Auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                    await context.HttpContext.Response.WriteAsync(
                        "Too many attempts. Please try again in a minute.", cancellationToken);
                options.AddPolicy("account-post", context => HttpMethods.IsPost(context.Request.Method)
                    ? RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 12,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        })
                    : RateLimitPartition.GetNoLimiter("browse"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
