using BuisnessLayer.Interface;
using BuisnessLayer.Service;
using BuisnessLayer.User.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middleware.ExceptionHandling;
using Middleware.Services.Email;
using Middleware.Services.JWT;
using NLog;
using NLog.Web;
using RepositoryLayer.context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using StackExchange.Redis;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        // Add services to the container.

        builder.Services.AddControllers(); //Meri application mein Controllers hain aur mujhe MVC/Web API controller services enable karni hain
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        // Swagger me "Authorize" button (token paste karne ke liye)
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste the token here"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserHandler).Assembly));
        builder.Services.AddScoped<IRegisterUserBL, RegisterUserBL>();
        builder.Services.AddScoped<IRegisterUserRL, RegisterUserRL>();
        builder.Services.AddScoped<INotesBL, NotesBL>();
        builder.Services.AddScoped<INotesRL, NotesRL>();
        builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
        builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
        builder.Services.AddDbContext<Fundoo2Context>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Fundoo2Connection"));
        });
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(builder.Configuration["Redis:ConnectionString"]!);
            options.AbortOnConnectFail = false;   // app still starts if Redis is down
            return ConnectionMultiplexer.Connect(options);
        });
        // JWT verification rules
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)),
                    ClockSkew = TimeSpan.Zero   // expiry bilkul time pe (default 5 min extra deta hai)
                };
            });
       

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        // Services me (Build() se pehle)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000")   // frontend ka address
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        


        var app = builder.Build(); //Build the configured application.

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseExceptionHandler();
        app.UseForwardedHeaders();

        // Pipeline me (Build() ke baad, UseAuthentication se pehle)
        app.UseCors("FrontendPolicy");

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();   // 1. "Tum kaun ho?" (token verify)
        app.UseAuthorization();    // 2. "Tumhe permission hai?" ([Authorize])

        app.MapControllers();

        app.Run();
    }
}
