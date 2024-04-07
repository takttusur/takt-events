using Quartz;
using StackExchange.Redis;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Infrastructure.FakeImplementations;
using TaktTusur.Media.Infrastructure.Jobs;
using TaktTusur.Media.Infrastructure.RedisRepository;
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
                c.WithCronSchedule(timetable.NewsReplicationJob).ForJob(newsJobKey);
            });
        });
        services.AddQuartzHostedService((options) =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        services.Configure<NewsReplicationJobConfiguration>(configuration.GetSection(nameof(NewsReplicationJobConfiguration)));
        services.AddSingleton<IArticlesRemoteSource, FakeArticleRemoteSource>();
        services.AddSingleton<ITextTransformer, TextTransformer>();
        services.AddSingleton<IEnvironment, EnvironmentService>();
        services.AddScoped<IArticlesRepository, FakeArticlesRepository>();
        services.AddSingleton<IConnectionMultiplexer>(options =>
            ConnectionMultiplexer.Connect("127.0.0.1:6379")
        );
    })
    .Build();

host.Run();
