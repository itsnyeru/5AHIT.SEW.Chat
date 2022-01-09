using Blazored.LocalStorage;
using Domain.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Configuration;
using Model.Entity;
using Services;
using System.Reflection;

namespace UI;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddUI(this IServiceCollection services, params object[] values) {
        Configuration configuration = new Configuration();

        services.AddDbContext<ChatDbContext>(
            options => options.UseMySql(configuration.DefaultConnection, new MySqlServerVersion(new System.Version("8.0.25")))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddSingleton(configuration.DefaultMail);
        services.AddScoped<IMailService, MailService>();

        services.AddSingleton(configuration.DefaultSms);
        services.AddScoped<ISmsService, VonageService>();

        services.AddBlazoredLocalStorage();
        services.AddHttpClient();
        services.AddScoped<AuthenticationStateProvider, AuthenticationService>();

        services.AddSignalR();

        /*
         configuration.GetConnectionString("DefaultConnection")
        configuration.GetSection("DefaultMail").Get<MailSettings>()
        configuration.GetSection("DefaultSms").Get<SmsSettings>()
         */

        return services;
    }

    public static string GetPath() => Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase ?? "").Path);

    public static IConfigurationRoot GetConfiguration() => new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), false).Build();

    #pragma warning disable CS8604 // Possible null reference argument.
    public async static Task InsertData(this IServiceProvider provider) {
        using(var scope = provider.CreateScope()) {
            //Create Database
            var _context = scope.ServiceProvider.GetService<ChatDbContext>();
            if(_context == null) return;
            //context.Database.Migrate();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            //Insert Data
            var _repository = scope.ServiceProvider.GetService<IUserRepository>();
            var _chatRepository = scope.ServiceProvider.GetService<IChatRepository>();
            if(_repository == null) return;
            if(_chatRepository == null) return;
            if(!(await _repository.FilterAsync(u => u.Id == 1)).Any()) {
                await _repository.CreateAsync(new User { Username = "Nico", Email = "n@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });
                await _repository.CreateAsync(new User { Username = "Patrick", Email = "p@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });
                await _repository.CreateAsync(new User { Username = "Tobias", Email = "t@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });
                await _repository.CreateAsync(new User { Username = "Simon", Email = "s@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });
                await _repository.CreateAsync(new User { Username = "Max", Email = "m@mail.com", Password = "Test", ChatMode = ChatMode.PUBLIC });
                await _repository.CreateAsync(new User { Username = "Daniel", Email = "d@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });
                await _repository.CreateAsync(new User { Username = "Rene", Email = "r@mail.com", Password = "Test", ChatMode = ChatMode.ONLY_FRIENDS });

                var nico = await _repository.GetAccount(1);
                var patrick = await _repository.GetAccount(2);
                var tobias = await _repository.GetAccount(3);
                var simon = await _repository.GetAccount(4);
                var max = await _repository.GetAccount(5);
                var daniel = await _repository.GetAccount(6);
                var rene = await _repository.GetAccount(7);


                var sc = await _chatRepository.CreateSinglechat(nico, max);

                await _repository.AddFriend(nico, daniel);
                await _repository.AddFriend(daniel, nico);
                var scnd = await _chatRepository.CreateSinglechat(nico, daniel);
                await _repository.AddFriend(nico, rene);
                await _repository.AddFriend(rene, nico);
                var scnr = await _chatRepository.CreateSinglechat(nico, rene);

                await _repository.AddFriend(simon, nico);
            }
        }
    }
    #pragma warning restore CS8604 // Possible null reference argument.
}
