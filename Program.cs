using System.Text;
using System.Text.Json;
using BlogAPI.Repository;
using BlogAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
builder.Services.AddSingleton(jwtKey);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.InvalidModelStateResponseFactory = context =>
  {
    var errors = context.ModelState
      .Where(e => e.Value?.Errors.Count > 0)
      .Select(e => new
      {
        Field = e.Key,
        Messages = e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
      });

    return new BadRequestObjectResult(new
    {
      message = "Validation failed",
      errors
    });
  };
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(options =>
{
  options.Filters.Add<ExceptionFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtKey
      };
      options.Events = new JwtBearerEvents
      {
        OnChallenge = context =>
        {
          context.HandleResponse();

          context.Response.StatusCode = 401;
          context.Response.ContentType = "application/json";

          var problem = new
          {
            title = "Unauthorized",
            status = 401,
            detail = "Invalid or missing token",
            instance = context.HttpContext.Request.Path
          };

          var json = JsonSerializer.Serialize(problem);
          return context.Response.WriteAsync(json);
        }
      };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();