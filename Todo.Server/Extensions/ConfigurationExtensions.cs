using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TodoServer.Application.Interfaces;
using TodoServer.Application.Mapping;
using TodoServer.Application.Services;
using TodoServer.Entities;
using TodoServer.Entities.Enums;
using TodoServer.Entities.Interfaces;
using TodoServer.Infrastructure.Data;
using TodoServer.Validators;

namespace TodoServer.Extensions
{
    internal static class ConfigurationExtensions
    {
        public static void AddDependencInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepository<Todo>, Repository<Todo>>();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<IValidator<Todo>, TodoValidator>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddValidatorsFromAssemblyContaining<TodoValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateTodoDtoValidator>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = false;
            }, typeof(Program).Assembly);
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=todoapp.db"));
        }

        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                logger.LogInformation("Applying pending migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully");

                logger.LogInformation("Seeding initial data...");
                await SeedDataAsync(context);
                logger.LogInformation("Data seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }


        private static async Task SeedDataAsync(ApplicationDbContext context)
        {
            if (!await context.Todos.AnyAsync())
            {
                var sampleTodos = new[]
                {
                    new Todo
                    {
                        Title = "Complete assessment",
                        Description = "Finish the .NET Core React todo app",
                        Tag = "work",
                        CreatedAt = DateTime.UtcNow,
                        DueDate = DateTime.UtcNow.AddMonths(1)
                    },
                    new Todo
                    {
                        Title = "Review code",
                        Description = "Check all components work properly",
                        Tag = "work",
                        CreatedAt = DateTime.UtcNow.AddHours(-1),
                        IsCompleted = true,
                         DueDate = DateTime.UtcNow.AddMonths(1)
                    },
                    new Todo
                    {
                        Title = "Prepare presentation",
                        Description = "Get ready to present your work",
                        Tag = "work",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        Priority = PriorityLevel.High,
                        DueDate = DateTime.UtcNow.AddMonths(1)
                    }
                };

                await context.Todos.AddRangeAsync(sampleTodos);
                await context.SaveChangesAsync();
            }
        }

        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Todo API",
                    Version = "v1",
                    Description = "API for managing todo items",
                    Contact = new OpenApiContact
                    {
                        Name = "Your Name",
                        Email = "your.email@example.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        public static void AllowCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("DynamicCorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

        }
    }
}
