using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using padelya_api.Data;
using padelya_api.Services;
using padelya_api.Services.Annual;
using padelya_api.Services.Annual.Scoring;
using padelya_api.Services.Notification;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
      {
        policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});

// Add services to the container.
builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.Converters.Add(
      new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
  });


//  options.JsonSerializerOptions.Converters.Add(
//       new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
//     );

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PadelYaDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
      };
    });

// Add Authorization with custom policy
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("RequirePermission", policy =>
      policy.RequireAuthenticatedUser());
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IComplexService, ComplexService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICourtSlotService, CourtSlotService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAnnualTableService, AnnualTableService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IScoringStrategy, ChallengeScoringStrategy>();
builder.Services.AddScoped<IScoringStrategy, TournamentScoringStrategy>();
builder.Services.AddScoped<IScoringStrategy, ClassScoringStrategy>();
builder.Services.AddScoped<IScoringStrategy, MatchWinScoringStrategy>();
builder.Services.AddScoped<IScoringStrategy, MatchLossScoringStrategy>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapScalarApiReference();
  app.MapOpenApi();
  app.UseCors("AllowAll");
}

//app.UseHttpsRedirection();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
