// create DB tables to load data
// parse and load data
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SongsDataLoaderApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
IConfiguration configuration = 
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

// IHostEnvironment env = builder.Environment;
// builder.Configuration
//     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

var optionsBuilder = new DbContextOptionsBuilder<SongsLoaderContext>();
optionsBuilder.UseSqlite(configuration.GetConnectionString("SongsData"));
// optionsBuilder.UseSqlite(@"DataSource=mydatabase.db;");


builder.Services
    .AddDbContext<SongsLoaderContext>(
        options =>
            options = optionsBuilder);

builder.Services.AddTransient<Repository>();
builder.Services.AddTransient<DataLoader>();

IHost host = builder.Build();

// var config = host.Services.GetRequiredService<IConfiguration>();

using (var context = host.Services.GetRequiredService<SongsLoaderContext>()){
    if(!context.GetService<IRelationalDatabaseCreator>().Exists()){
        context.Database.EnsureCreated();
    }


    DataLoader dataLoader = host.Services.GetRequiredService<DataLoader>();
    dataLoader.LoadReferenceData();
    DataLoaderOptions options = new DataLoaderOptions();
    configuration.GetSection(DataLoaderOptions.LoaderSettings).Bind(options);
    dataLoader.ParseAndLoadSongs(options.FilePath, options.BatchSize);
    ILogger logger = host.Services.GetRequiredService<ILogger>();
    logger.LogInformation("******Exiting");
}

host.RunAsync();

