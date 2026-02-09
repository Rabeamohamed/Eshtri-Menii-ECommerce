using ECom.API.Middleware;
using ECom.Infrastructure;
var builder = WebApplication.CreateBuilder(args);



// CORS Policy Configuration 
builder.Services.AddCors(op =>
{
    op.AddPolicy("CROSPolicy", builder =>
    {
        builder.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("http://localhost:4200"); // Adjust the origin as needed
    });
}); 
// In-Memory Caching Service Registration for Rate Limiting in Exception Middlewaret
builder.Services.AddMemoryCache();

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

app.UseCors("CROSPolicy"); // CORS Middleware Registration

app.UseMiddleware<ExceptionMiddleware>(); // Custom Exception Middleware Registration

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Global Error Handling Middleware Registration to Redirect to Error Controller

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
