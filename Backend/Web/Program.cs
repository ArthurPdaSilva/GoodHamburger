
using Application.Mapping;
using Application.Services;
using Application.Services.Interfaces;
using Domain;
using Domain.Repositories;
using Domain.Repositories.Interfaces;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Mesmo virando pago, enquanto não subir para produção, o AutoMapper continua gratuito, então vou continuar usando ele para facilitar o mapeamento entre as entidades e os DTOs.
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingContext>());
            //Tive que adicionar essa configuração para o Entity Framework Core reconhecer o PostgreSQL como banco de dados.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                // o UseNpgsql é a configuração necessária para utilizar o postgresql.
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Estou configurando o Header do Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Good Hamburguer API", Version = "v1" });
            });

            //Realizando as injeções de dependênciasa aqui:
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();
            builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

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
            app.UseCors("AllowSpecificOrigin");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
