using Communication;
using Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddFacebook(options => {
                options.AppId = Configuration["AuthenticationProviders:Facebook:AppId"];
                options.AppSecret = Configuration["AuthenticationProviders:Facebook:AppSecret"];
            })
            .AddGoogle(options => {
                options.ClientId = Configuration["AuthenticationProviders:Google:ClientId"];
                options.ClientSecret = Configuration["AuthenticationProviders:Google:ClientSecret"];
            })
            .AddCookie(options => {
                options.Cookie.Name = "CustomAuth";
                options.Cookie.Domain = ".ctsbaltic.com";
            });

            services.AddCors(); 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IVerificationManager, VerificationManager>();
            services.AddSingleton<IVerificationCodeManager, VerificationCodeManager>();
            services.AddSingleton<IEmailManager, EmailManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {                
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                // Fix redirecturi to be HTTPS when logging in
                app.Use((context, next) =>
                {
                    if (context.Request.Headers["x-forwarded-proto"] == "https")
                    {
                        context.Request.Scheme = "https";
                    }
                    return next();
                });
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            
            app.UseCors(
                options => options.WithOrigins("https://ctsbaltic.com", "https://*.ctsbaltic.com")
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
            );
            
            app.UseMvc();
        }
    }
}
