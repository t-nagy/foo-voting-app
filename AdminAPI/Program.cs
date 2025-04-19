
using AdminAPI.Controllers.Data;
using AdminAPI.DataAccess;
using AdminAPI.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Cryptography;
using System.Xml;

namespace AdminAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<DataContext>()
                .AddApiEndpoints();

            builder.Services.AddOptions<IdentityOptions>().Configure(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });

            builder.Services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme).Configure(options => {
                options.BearerTokenExpiration = TimeSpan.FromSeconds(300);
            });

            builder.Services.AddTransient<KeyService>();
            builder.Services.AddTransient<ConfigHelper>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IPollOptionData, PollOptionSqlData>();
            builder.Services.AddTransient<IParticipantData, ParticipantSqlData>();
            builder.Services.AddTransient<IKeyData, KeySqlData>();
            builder.Services.AddTransient<IPollData, PollSqlData>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapIdentityApi<IdentityUser>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
