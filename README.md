# abp_net8_webapi

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IMFG.ADA.EntityFrameworkCore;
using IMFG.ADA.MultiTenancy;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;
using Hangfire;
using Volo.Abp.BackgroundJobs;
using Hangfire.PostgreSql;
using Dapper;
using IMFG.ADA.Helpers;
using Volo.Abp.Hangfire;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;
using Polly;
using ADACore.Storage;
using ADACore.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Azure;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.BlobStoring.Aws;

namespace IMFG.ADA;

[DependsOn(
    typeof(ADAHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(ADAApplicationModule),
    typeof(ADAEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
[DependsOn(typeof(AbpAspNetCoreSignalRModule))]
    [DependsOn(typeof(AbpBlobStoringModule))]
    [DependsOn(typeof(AbpBlobStoringAzureModule))]
    [DependsOn(typeof(AbpBlobStoringFileSystemModule))]
    [DependsOn(typeof(AbpBlobStoringMinioModule))]
    [DependsOn(typeof(AbpBlobStoringAwsModule))]
public class ADAHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureCache();
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        ConfigureHangfire(context, configuration);
        ConfigureSignalR(context);
        ConfigureBlobStore(configuration);
    }

    private void ConfigureCache()
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "ADA:"; });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ADADomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}IMFG.ADA.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<ADADomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}IMFG.ADA.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<ADAApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}IMFG.ADA.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<ADAApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}IMFG.ADA.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ADAApplicationModule).Assembly);
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                options.Audience = "ADA";
            });

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                    {"ADA", "ADA API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ADA API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("ADA");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "ADA-Protection-Keys");
        }
    }

    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
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

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddHangfire(config =>
        {
            config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration.GetConnectionString("ADACore"),
             new PostgreSqlStorageOptions()
             {
                 InvisibilityTimeout = TimeSpan.FromMinutes(5),
                 QueuePollInterval = TimeSpan.FromMilliseconds(200),
                 DistributedLockTimeout = TimeSpan.FromMinutes(1),
                 PrepareSchemaIfNecessary = true,
                 SchemaName = "hangfire"
             });
            SqlMapper.AddTypeHandler(new DapperDateTimeTypeHandler());
        });

        Configure<AbpHangfireOptions>(options =>
        {
            //options.ServerOptions.ServerName = "YourProjectNameServer";
            options.ServerOptions = new BackgroundJobServerOptions
            {
                ServerName = configuration["Hangfire:ServerName"],
                WorkerCount = Environment.ProcessorCount * 5, //Convert.ToInt32(configuration["Hangfire:WorkerCount"]), // Set the number of workers
                Queues = new[] { "alpha", "beta", "default" }, // Define queues
                //SchedulePollingInterval = TimeSpan.FromSeconds(Convert.ToInt32(configuration["Hangfire:SchedulePollingInterval"])), // Set polling interval
                //HeartbeatInterval = TimeSpan.FromMinutes(Convert.ToInt32(configuration["Hangfire:HeartbeatInterval"])), // Set heartbeat interval
                //ServerCheckInterval = TimeSpan.FromMinutes(Convert.ToInt32(configuration["Hangfire:ServerCheckInterval"])) // Set server check interval
            };
        });


    }

    private void ConfigureSignalR(ServiceConfigurationContext context)
    {
        context.Services.AddSignalR();
        context.Services.AddTransient<SignalRHub>();
    }

    private void ConfigureBlobStore(IConfiguration configuration)
    {
        string BlobProvider = configuration["BlobStore:Provider"];

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<BlobStorageContainer>(container =>
            {
                container.IsMultiTenant = false;

                switch (BlobProvider.ToUpper())
                {
                    case "PHYSICAL":
                        container.UseFileSystem(fileSystem =>
                        {
                            fileSystem.BasePath = configuration["BlobStore:Physical:BasicPath"];
                        });
                        break;
                    case "AZURE":
                        string azureConnectionString = string.Format("{0}{1}{2}{3}{4}",
                            configuration["BlobStore:Azure:BlobEndpoint"],
                            configuration["BlobStore:Azure:QueueEndpoint"],
                            configuration["BlobStore:Azure:FileEndpoint"],
                            configuration["BlobStore:Azure:TableEndPoint"],
                            configuration["BlobStore:Azure:SharedEndPoint"]);
                        container.UseAzure(azure =>
                        {
                            azure.ConnectionString = azureConnectionString;
                            azure.ContainerName = configuration["BlobStore:Azure:ContainerName"];
                            azure.CreateContainerIfNotExists = Convert.ToBoolean(configuration["BlobStore:Azure:CreateContainerIfNotExits"]);
                        });
                        break;
                    case "AWS":
                        container.UseAws(Aws =>
                        {
                            Aws.AccessKeyId = configuration["BlobStore:Aws:AccessKeyId"];
                            Aws.SecretAccessKey = configuration["BlobStore:Aws:AccessKeyId"];
                            Aws.UseCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseCredentials"]);
                            Aws.UseTemporaryCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseTemporaryCredentials"]);
                            Aws.UseTemporaryFederatedCredentials = Convert.ToBoolean(configuration["BlobStore:Aws:UseTemporaryFederatedCredentials"]);
                            Aws.ProfileName = configuration["BlobStore:Aws:ProfileName"];
                            Aws.ProfilesLocation = configuration["BlobStore:Aws:ProfilesLocation"];
                            Aws.Region = configuration["BlobStore:Aws:Region"];
                            Aws.Policy = configuration["BlobStore:Aws:Policy"];
                            Aws.DurationSeconds = Convert.ToInt32(configuration["BlobStore:Aws:DurationSeconds"]);
                            Aws.ContainerName = configuration["BlobStore:Aws:ContainerName"];
                            Aws.CreateContainerIfNotExists = Convert.ToBoolean(configuration["BlobStore:Aws:CreateContainerIfNotExists"]);
                        });
                        break;
                    case "MINIO":
                        container.UseMinio(minio =>
                        {
                            minio.EndPoint = configuration["BlobStore:Minio:EndPoint"];
                            minio.AccessKey = configuration["BlobStore:Minio:AccessKey"];
                            minio.SecretKey = configuration["BlobStore:Minio:SecretKey"];
                            minio.BucketName = configuration["BlobStore:Minio:BucketName"];
                            minio.WithSSL = true;
                        });
                        break;
                    default:
                        break;
                }
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var configuration = context.GetConfiguration();
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

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
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ADA API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("ADA");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseAbpHangfireDashboard("/hangfire"); //should add to the request pipeline before the app.UseConfiguredEndpoints()
        //app.UseConfiguredEndpoints();

        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHub<SignalRHub>("/" + configuration["SignalR:HubName"], options =>
            {
                options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
            });
        });
    }

}
