using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.IdentifyDataLocks.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PaymentsDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PaymentsSqlConnectionString"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddScoped<IPaymentsDataContext, PaymentsDataContext>();

            services.AddRazorPages(options => 
            {
                options.Conventions.AllowAnonymousToPage("/index");
                options.Conventions.AuthorizePage("/accessdenied");
                //This redirection is required as IDAMs is configured to return to this path for localhost. 
                options.Conventions.AddPageRoute("/accessdenied", "account/accessdenied");
                options.Conventions.AuthorizePage("/start", AuthorizationConfiguration.PolicyName);
                options.Conventions.AuthorizePage("/learner", AuthorizationConfiguration.PolicyName);
            });
            var authorizationConfig = Configuration.GetSection("Authorization").Get<AuthorizationConfiguration>();
            var authenticationConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfiguration>();
            services.AddAuthentication(authenticationConfig);
            services.AddAuthorization(authorizationConfig);
            services.Configure<HtmlHelperOptions>(o => o.ClientValidationEnabled = false);
            RegisterServices(services);
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/ping");
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            var commitmentsClientApiConfiguration = new CommitmentsClientApiConfiguration();
            Configuration.GetSection(nameof(CommitmentsClientApiConfiguration)).Bind(commitmentsClientApiConfiguration);
            services.AddSingleton(commitmentsClientApiConfiguration);
            services.AddSingleton<ICommitmentsApiClientFactory, CommitmentsApiClientFactory>();
            services.AddTransient(x => x.GetService<ICommitmentsApiClientFactory>().CreateClient());
            services.AddTransient<CommitmentsService>();

            var accountsApiConfiguration = new AccountApiConfiguration();
            Configuration.GetSection(nameof(AccountApiConfiguration)).Bind(accountsApiConfiguration);
            services.AddSingleton<IAccountApiConfiguration>(accountsApiConfiguration);
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddTransient<EmployerService>();

            services.AddTransient<IProviderApiClient>((x) => new ProviderApiClient(Configuration.GetValue<string>("FatApiBaseUri")));
            services.AddTransient<ProviderService>();
            services.AddTransient<DataLockService>();
            services.AddTransient<LearnerReportProvider>();
            services.AddTransient<ITimeProvider, TimeProvider>();
        }
    }
}
