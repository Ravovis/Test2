
namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // CORS for local Angular dev on 4200
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontCors", policy =>
                    policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
            });

            // In-memory store and services
            builder.Services.AddSingleton<WebApplication1.Services.IInMemoryStore, WebApplication1.Services.InMemoryStore>();
            builder.Services.AddSingleton<WebApplication1.Services.ISeedData, WebApplication1.Services.SeedData>();
            builder.Services.AddSingleton<WebApplication1.Services.ITaskService, WebApplication1.Services.TaskService>();
            builder.Services.AddSingleton<WebApplication1.Services.IUserService, WebApplication1.Services.UserService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("FrontCors");
            app.UseAuthorization();


            app.MapControllers();

            // Seed after app built so singletons exist
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<WebApplication1.Services.ISeedData>();
                seeder.EnsureSeeded();
            }

            app.Run();
        }
    }
}
