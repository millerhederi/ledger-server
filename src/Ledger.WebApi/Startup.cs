using System.Reflection;
using System.Text;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Filters;
using Ledger.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Ledger.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void ConfigureIoc(IServiceCollection services)
        {
            services.AddTransient<IAuthenticateUserService, AuthenticateUserService>();
            services.AddTransient<IGetTransactionService, GetTransactionService>();
            services.AddTransient<IListTransactionsService, ListTransactionsService>();
            services.AddTransient<IUpsertTransactionService, UpsertTransactionService>();
            services.AddTransient<IListAccountsService, ListAccountsService>();
            services.AddTransient<IAddUserService, AddUserService>();
            services.AddTransient<IUpsertAccountService, UpsertAccountService>();
            services.AddTransient<IListPostingsService, ListPostingsService>();
            services.AddTransient<IGetPostingTotalsByMonthService, GetPostingTotalsByMonthService>();
            services.AddSingleton<IRepository, Repository>();
            services.AddScoped<IRequestContext, RequestContext>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddCors(options => { options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<RequestContextFilter>();
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());

            ConfigureIoc(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
