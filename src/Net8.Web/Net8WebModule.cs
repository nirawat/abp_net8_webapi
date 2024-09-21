using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net8.EntityFrameworkCore;
using Net8.Localization;
using Net8.MultiTenancy;
using Net8.Permissions;
using Net8.Web.Menus;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Studio;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Identity.Web;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp.TenantManagement.Web;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Identity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Studio.Client.AspNetCore;
using Net8.CoreService.Web;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using Volo.Abp.BlobStoring;
using Hangfire;
using Volo.Abp.Hangfire;
using Dapper;
using Hangfire.PostgreSql;
using Net8.Web.Helpers;
using Net8.CoreService.SignalR;
using Net8.CoreService.Storage;
using Volo.Abp.BackgroundJobs.Hangfire;

namespace Net8.Web;

[DependsOn(
    typeof(Net8HttpApiModule),
    typeof(Net8ApplicationModule),
    typeof(Net8EntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
[DependsOn(
    typeof(CoreServiceWebModule),
    typeof(AbpBackgroundJobsHangfireModule)
    )]
public class Net8WebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(Net8Resource),
                typeof(Net8DomainModule).Assembly,
                typeof(Net8DomainSharedModule).Assembly,
                typeof(Net8ApplicationModule).Assembly,
                typeof(Net8ApplicationContractsModule).Assembly,
                typeof(Net8WebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Net8");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", "9edc94a1-236d-4939-8e7f-8a0daea5efe2");
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
        }

        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureAuthentication(context);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context.Services);
        ConfigureHangfire(context, configuration);
        ConfigureSignalR(context);
        ConfigureBlobStore(configuration);

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
        });
    }


    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<Net8WebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<Net8WebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<Net8DomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Net8.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<Net8DomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Net8.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<Net8ApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Net8.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<Net8ApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Net8.Application", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<Net8HttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Net8.HttpApi", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<Net8WebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new Net8MenuContributor());
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new Net8ToolbarContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(Net8ApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Net8 API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddHangfire(config =>
        {
            config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration.GetConnectionString("Default"),
             new PostgreSqlStorageOptions()
             {
                 InvisibilityTimeout = TimeSpan.FromMinutes(5),
                 QueuePollInterval = TimeSpan.FromMilliseconds(200),
                 DistributedLockTimeout = TimeSpan.FromMinutes(1),
                 PrepareSchemaIfNecessary = true,
                 UseNativeDatabaseTransactions = true,
                 SchemaName = "hangfire"
             });
            SqlMapper.AddTypeHandler(new DapperDateTimeTypeHandler());
        });

        Configure<AbpHangfireOptions>(options =>
        {
            //options.ServerOptions.ServerName = "YourProjectNameServer";
            options.ServerOptions = new BackgroundJobServerOptions
            {
                ServerName = configuration["Hangfire:Server"],
                WorkerCount = Environment.ProcessorCount * 5, //Convert.ToInt32(configuration["Hangfire:WorkerCount"]), // Set the number of workers
                Queues = new[] { "alpha", "beta", "default" }, // Define queues
            };
        });

        // Add Hangfire server
        context.Services.AddHangfireServer();
    }

    private void ConfigureSignalR(ServiceConfigurationContext context)
    {
        context.Services.AddSignalR();
        context.Services.AddTransient<SignalRHub>();
    }

    private void ConfigureBlobStore(IConfiguration configuration)
    {
        string BlobProvider = configuration["BlobStore:Provider"];

        //Configure<AbpBlobStoringOptions>(options =>
        //{
        //    options.Containers.Configure<BlobStorageConntainer>(container =>
        //    {
        //        container.IsMultiTenant = false;

        //        switch (BlobProvider.ToUpper())
        //        {
        //            case "PHYSICAL":
        //                container.UseFileSystem(fileSystem =>
        //                {
        //                    fileSystem.BasePath = configuration["BlobStore:Physical:BasicPath"];
        //                });
        //                break;
        //            case "AZURE":
        //                string azureConnectionString = string.Format("{0}{1}{2}{3}{4}",
        //                    configuration["BlobStore:Azure:BlobEndpoint"],
        //                    configuration["BlobStore:Azure:QueueEndpoint"],
        //                    configuration["BlobStore:Azure:FileEndpoint"],
        //                    configuration["BlobStore:Azure:TableEndPoint"],
        //                    configuration["BlobStore:Azure:SharedEndPoint"]);
        //                container.UseAzure(azure =>
        //                {
        //                    azure.ConnectionString = azureConnectionString;
        //                    azure.ContainerName = configuration["BlobStore:Azure:ContainerName"];
        //                    azure.CreateContainerIfNotExists = Convert.ToBoolean(configuration["BlobStore:Azure:CreateContainerIfNotExits"]);
        //                });
        //                break;
        //            case "AWS":
        //                container.UseAws(Aws =>
        //                {
        //                    Aws.AccessKeyId = configuration["BlobStore:Aws:AccessKeyId"];
        //                    Aws.SecretAccessKey = configuration["BlobStore:Aws:AccessKeyId"];
        //                    Aws.UseCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseCredentials"]);
        //                    Aws.UseTemporaryCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseTemporaryCredentials"]);
        //                    Aws.UseTemporaryFederatedCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseTemporaryFederatedCredentials"]);
        //                    Aws.ProfileName = configuration["BlobStore:Aws:ProfileName"];
        //                    Aws.ProfilesLocation = configuration["BlobStore:Aws:ProfilesLocation"];
        //                    Aws.Region = configuration["BlobStore:Aws:Region"];
        //                    Aws.Policy = configuration["BlobStore:Aws:Policy"];
        //                    Aws.DurationSeconds = Convert.ToInt32(configuration["BlobStore:Aws:DurationSeconds"]);
        //                    Aws.ContainerName = configuration["BlobStore:Aws:ContainerName"];
        //                    Aws.CreateContainerIfNotExists = Convert.ToBoolean(configuration["BlobStore:Aws:CreateContainerIfNotExists"]);
        //                });
        //                break;
        //            case "MINIO":
        //                container.UseMinio(minio =>
        //                {
        //                    minio.EndPoint = configuration["BlobStore:Minio:EndPoint"];
        //                    minio.AccessKey = configuration["BlobStore:Minio:AccessKey"];
        //                    minio.SecretKey = configuration["BlobStore:Minio:SecretKey"];
        //                    minio.BucketName = configuration["BlobStore:Minio:BucketName"];
        //                    minio.WithSSL = true;
        //                });
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //});
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var conf = context.GetConfiguration();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseAbpStudioLink();
        app.UseRouting();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Net8 API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseAbpHangfireDashboard("/hangfire");
        //app.UseConfiguredEndpoints();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHub<SignalRHub>("/" + conf["SignalR:Hub"], options =>
            {
                options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
            });
        });
    }
}
