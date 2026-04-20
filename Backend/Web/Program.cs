
using Domain;
using Domain.Repositories;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Tive que adicionar essa configuração para o Entity Framework Core reconhecer o PostgreSQL como banco de dados.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                // o UseNpgsql é a configuração necessária para utilizar o postgresql.
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            //Realizando as injeções de dependênciasa aqui:
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Adicionando cors e permitindo o acesso do meu frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
