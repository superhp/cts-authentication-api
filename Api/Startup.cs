using Communication;
using Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;

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
        public async System.Threading.Tasks.Task ConfigureServicesAsync(IServiceCollection services)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var token = await tokenProvider.GetAccessTokenAsync("https://storage.azure.com/");
            var credentials = new StorageCredentials(new TokenCredential(token));
            var storageAccount = new CloudStorageAccount(credentials, "ctsinternalstorage", "core.windows.net", useHttps: true);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("my-key-container");

            // optional - provision the container automatically
            await container.CreateIfNotExistsAsync();

            services.AddDataProtection()
                .SetApplicationName("CtsBaltic")
                .PersistKeysToAzureBlobStorage(container, "keys.xml");
                //.PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"C:\Dev\test\keys.txt"));
                //.PersistKeysToAzureBlobStorage(new Uri("https://ctsinternalstorage.blob.core.windows.net/ctsinternalauthblob/keys.txt?sp=rcwd&st=2019-07-14T18:16:53Z&se=2019-07-15T02:16:53Z&spr=https&sv=2018-03-28&sig=SOZxrECXA2cCs3SaESMHtN%2FkjADBG0PsXNAxu1FkBl4%3D&sr=b"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddFacebook(options =>
            {
                options.AppId = Configuration["AuthenticationProviders:Facebook:AppId"];
                options.AppSecret = Configuration["AuthenticationProviders:Facebook:AppSecret"];
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["AuthenticationProviders:Google:ClientId"];
                options.ClientSecret = Configuration["AuthenticationProviders:Google:ClientSecret"];
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "CustomAuth";
                options.Cookie.Domain = ".ctsbaltic.com";
            });

            services.AddCors(options =>
                options.AddPolicy("AllowSubdomain",
                    builder =>
                    {
                        builder.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                )
            );

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
                app.UseDeveloperExceptionPage();
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
                options => options.WithOrigins("https://ctsbaltic.com")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader()
            );

            app.UseMvc();
        }
    }
}
