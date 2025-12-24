var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// builder.Services.AddOpenApi(); // Removing to avoid conflict
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Core Services
builder.Services.AddSingleton<Banking.Core.Interfaces.IAccountService, Banking.Core.Services.AccountService>();
builder.Services.AddSingleton<List<Banking.Core.Entities.Account>>(); // Simple In-Memory DB

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); // Removing to avoid conflict
    app.UseSwagger();
    app.UseSwaggerUI(); // serves at /swagger
}



app.UseHttpsRedirection();

app.UseDefaultFiles(); // Serve index.html by default
app.UseStaticFiles();  // Serve files from wwwroot

app.UseAuthorization();

app.MapControllers();

app.Run();
