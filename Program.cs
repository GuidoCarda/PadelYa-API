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
using padelya_api.Services.Email;
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

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(
      new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

  });


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PadelYaDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHttpContextAccessor();

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


// Configuración y servicios de Email (deben registrarse antes de los servicios que los usan)
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IComplexService, ComplexService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICourtSlotService, CourtSlotService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IClassTypeService, ClassTypeService>();
builder.Services.AddScoped<ILessonEnrollmentService, LessonEnrollmentService>();
builder.Services.AddScoped<ILessonAttendanceService, LessonAttendanceService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<ILessonRoutineAssignmentService, LessonRoutineAssignmentService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IBracketGenerationService, BracketGenerationService>();
builder.Services.AddScoped<IMatchSchedulingService, MatchSchedulingService>();
builder.Services.AddScoped<IMatchResultService, MatchResultService>();
builder.Services.AddScoped<IAutoSchedulingService, AutoSchedulingService>();
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
builder.Services.AddScoped<IRankingTraceService, RankingTraceService>();
builder.Services.AddScoped<IRepairService, RepairService>();

// Servicio de background para actualizar estados de torneos automáticamente
builder.Services.AddHostedService<padelya_api.Services.TournamentStatusUpdateService>();


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
