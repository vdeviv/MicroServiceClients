
using Clients.Application.Interfaces; 
using Clients.Application.Services;
using Clients.Application.Validators; 
using Clients.Domain.Entities;
using Clients.Domain.Interfaces; 
using Clients.Infrastructure.Persistences; 
using Clients.Infrastructure.Repository; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

DatabaseConnection.Initialize(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepository<Client>, ClientRepository>();

builder.Services.AddScoped<IValidator<Client>, ClientValidator>();

builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddHttpClient("clienteApi", c => {

    c.BaseAddress = new Uri("http://localhost:5142");
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();