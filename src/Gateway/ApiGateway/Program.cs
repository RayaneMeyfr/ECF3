using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseCors("AllowBlazor");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/books/swagger/v1/swagger.json", "Catalog Service");
    c.SwaggerEndpoint("/users/swagger/v1/swagger.json", "User Service");
    c.SwaggerEndpoint("/loans/swagger/v1/swagger.json", "Loan Service");
});

await app.UseOcelot();

app.Run();
