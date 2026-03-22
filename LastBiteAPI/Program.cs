using Microsoft.EntityFrameworkCore;
using LastBiteAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// make it accessible to local computers
builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<LastBiteDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Supabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowNextApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
