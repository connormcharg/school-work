using backend.Classes.Data;
using backend.Hubs;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            var keyString = "z&p^%3cz5#ineEVdW!3aAj";
            var key = Encoding.ASCII.GetBytes(keyString);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddScoped<IAuthenticationService>(sp =>
            {
                return new AuthenticationService(
                    sp.GetRequiredService<IUserRepository>(),
                    keyString,
                    30);
            });

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<ChessService>();
            builder.Services.AddSingleton<ConnectionMappingService>();
            builder.Services.AddHostedService<TimerService>();
            // need to add the engine service

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChessHub>("/chesshub");
            app.MapControllers();

            app.Run();
        }
    }
}
