using CardGame.Shared.Models;
using CardGame.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQLite
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

// CORS konfigūracija
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5103", "https://localhost:5103") // leisti abu variantus
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

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
app.Run();
