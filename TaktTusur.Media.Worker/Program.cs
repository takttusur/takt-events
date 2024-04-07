using System.Configuration;
using Quartz;
using StackExchange.Redis;
using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;
using TaktTusur.Media.Core.Resources;
using TaktTusur.Media.Core.Settings;
using TaktTusur.Media.Infrastructure.FakeImplementations;
using TaktTusur.Media.Infrastructure.Jobs;
using TaktTusur.Media.Infrastructure.RedisRepository;
using TaktTusur.Media.Infrastructure.Serializers;
using TaktTusur.Media.Infrastructure.Services;
using TaktTusur.Media.Worker.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context,services) =>
    {
        var configuration = context.Configuration;
        var timetable = configuration.GetTimetable();
        
        var newsJobKey = JobKey.Create(nameof(ArticlesReplicationJob));
        services.AddQuartz((options) =>
        {
            options.AddJob<ArticlesReplicationJob>(newsJobKey, configurator =>
            {
                configurator.DisallowConcurrentExecution();
            }).AddTrigger(c =>
            {
                c.WithCronSchedule(timetable.ArticlesReplicationJob).ForJob(newsJobKey);
            });
        });
        services.AddQuartzHostedService((options) =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        services.Configure<ReplicationJobConfiguration>(
            nameof(ArticlesReplicationJob), 
            configuration.GetSection(nameof(ArticlesReplicationJob)));
        services.Configure<TextRestrictions>(
            nameof(ArticlesReplicationJob),
    		configuration.GetSection(nameof(ArticlesReplicationJob)));
        
        services.AddSingleton<IArticlesRemoteSource, FakeArticleRemoteSource>();
        services.AddSingleton<ITextTransformer, TextTransformer>();
        services.AddSingleton<IEnvironment, EnvironmentService>();
        services.AddScoped<IArticlesRepository, ArticlesRedisRepository>();
        
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnectionString)) 
            throw new ConfigurationErrorsException(string.Format(Localization.CONNECTION_STRING_NOT_FOUND, "Redis"));
        
        services.AddSingleton<IConnectionMultiplexer>(options =>
            ConnectionMultiplexer.Connect(redisConnectionString)
        );
        services.AddSingleton<IJsonSerializer<DatedBucket<Article>>, DatedBucketSerializer<Article>>();
    })
    .Build();

host.Run();
