using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using Microsoft.OpenApi.Models;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var allowOrigins = "AllowOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowOrigins, policy =>
    {
         policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rock Paper Scissors API",
        Description = "An API based on a Rock Paper Scissors Game with Database Communication",
        Version = "v1" });
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPlayerInviteService, PlayerInviteService>();
builder.Services.AddScoped<IPlayerMovesService, PlayerMovesService>();
builder.Services.AddDbContext<DataContext>(
  options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated for Login", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.AuthenticationMethod, "Authenticated for Login");
    });
    options.AddPolicy("Authentication for Generating a New Password", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.AuthenticationMethod, "Authentication for Generating a New Password");
    });
});


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(2);
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/test", () => "Welcome to Rock Paper Scissors API");

#region Minimal API-Styles

app.MapGet("/minimalapi/users", async (DataContext context) => await
context.User.ToListAsync());

app.MapGet("/minimalapi/users/{id}", async (DataContext context, long UserID) => await
context.User.FindAsync(UserID) is User user ? Results.Ok(user) :
Results.NotFound("User could not be found."));

app.MapPost("/minimalapi/users/create", async (DataContext context, User user) =>
{
    context.User.Add(user);
    await context.SaveChangesAsync();
    return Results.Created($"/api/users/{user.UserID}", user);
});

app.MapPut("minimalapi/users/update/{id}", async (DataContext context, User user) =>
{
    context.User.Update(user);
    await context.SaveChangesAsync();
    return Results.Accepted($"/api/users/{user.UserID}", user);
});

app.MapDelete("minimalapi/users/delete/{id}", async (DataContext context, long UserID) =>
{
    if (await context.User.FindAsync(UserID) is User user)
    {
        context.User.Remove(user);
        await context.SaveChangesAsync();
        return Results.Ok(user);
    }

    return Results.NotFound();
});



#endregion

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
