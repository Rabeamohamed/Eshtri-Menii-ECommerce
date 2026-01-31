using ECom.API.Middleware;
using ECom.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.InfrastructureConfiguration(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // AutoMapper Configuration Registration Dependency Injection

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>(); // Custom Exception Middleware Registration

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Global Error Handling Middleware Registration to Redirect to Error Controller

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
