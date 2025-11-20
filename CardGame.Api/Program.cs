using CardGame.Shared.Models;
using CardGame.Server.Data;
using Microsoft.EntityFrameworkCore;
using CardGame.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();   // <-- Registruoja MVC controllerius
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQLite
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

// SignalR realaus laiko ryšiui 
builder.Services.AddSignalR();

// CORS konfigūracija   (leidžia Blazor frontend'ui jungtis prie šio API per SignalR)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
        policy.WithOrigins("https://localhost:5103")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // reikia SignalRui
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// naudoja CORS politika
app.UseCors("AllowBlazorDev");
app.UseHttpsRedirection();

// registruoja signalR hub
app.MapHub<GameHub>("/gamehub");

// GET root
app.MapGet("/", () => "CardGame API veikia!");

// POST registracija
app.MapPost("/users", async (User user, UserDbContext db) =>
{
    // Patikrinam ar jau yra toks email
    var existingEmail = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
    if (existingEmail != null)
        return Results.BadRequest(new { field = "Email", message = "This email is already registered." });

    // Patikrinam ar jau yra toks vartotojo vardas
    var existingUserName = await db.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
    if (existingUserName != null)
        return Results.BadRequest(new { field = "UserName", message = "This username is already taken." });

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.UserName}", user);
});


// GET visiems vartotojams
app.MapGet("/users", async (UserDbContext db) =>
    await db.Users.ToListAsync())
    .WithName("GetUsers")
    .WithOpenApi();
// POST login
app.MapPost("/login", async (User loginUser, UserDbContext db) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.Password == loginUser.Password);

    if (user == null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(user);
});
app.MapControllers();   // <-- Sujungia controller routes

app.Run();
