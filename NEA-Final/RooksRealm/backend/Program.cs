namespace backend
{
    using backend.Classes.Data;
    using backend.Hubs;
    using backend.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<Program>();
            var postgresPassword = builder.Configuration["postgres_password"];
            var connectionString = $"Host=mcharg.uk:5432;Database=rooksrealm;Username=root;Password={postgresPassword}";

            builder.Configuration["ConnectionString"] = connectionString;

            builder.Services.AddControllers();
            builder.Services.AddSignalR(options =>
            {

            })
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            var keyString = builder.Configuration["JwtKey"];
            if (keyString == null) { keyString = "AAAAA"; }
            var key = Encoding.UTF8.GetBytes(keyString);
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
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chesshub")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddScoped<IAuthenticationService>(sp =>
            {
                return new AuthenticationService(
                    sp.GetRequiredService<IUserRepository>(),
                    keyString,
                    1);
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.WithOrigins("http://localhost:5182")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddSingleton<ChessService>();
            builder.Services.AddSingleton<ConnectionMappingService>();
            builder.Services.AddHostedService<TimerService>();
            builder.Services.AddHostedService<EngineService>();
            builder.Services.AddHostedService<GameOverService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChessHub>("/chesshub");
            app.MapControllers();

            app.Run();
        }
    }
}
