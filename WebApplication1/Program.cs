using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração direta do banco de dados PostgreSQL
var databaseConfig = new DatabaseConfig
{
    Host = "localhost",
    Database = "multasdb",
    Username = "postgres",
    Password = "postgres",
    Port = 5432,
    SslMode = false
};

// Registrar DatabaseConfig como singleton para uso em toda a aplicação
builder.Services.AddSingleton(databaseConfig);

// Configurar DbContext com a configuração direta
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(databaseConfig.GetConnectionString()));

// Register repositories
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICondutorRepository, CondutorRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<ITipoMultaRepository, TipoMultaRepository>();
builder.Services.AddScoped<IMultaRepository, MultaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();