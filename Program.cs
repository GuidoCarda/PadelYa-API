using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using padelya_api.Data;
using System.IO;
using Microsoft.Extensions.FileProviders;
using padelya_api.Services;
using padelya_api.Services.Annual;
using padelya_api.Services.Annual.Scoring;
using padelya_api.Services.Email;
using padelya_api.Services.Notification;
using padelya_api.Services.Product;
using padelya_api.Services.Category;
using padelya_api.Services.File;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
      {
        policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("*");
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
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFileService, FileService>();

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

// Middleware para manejar Private Network Access (PNA)
app.Use(async (context, next) =>
{
  // Agregar header para permitir acceso desde redes públicas a privadas
  if (context.Request.Headers.ContainsKey("Access-Control-Request-Private-Network"))
  {
    context.Response.Headers.Add("Access-Control-Allow-Private-Network", "true");
  }

  await next();
});

//app.UseHttpsRedirection();

// Servir archivos estáticos (imágenes)
app.UseStaticFiles();

// Asegurar que exista la carpeta final para imágenes: wwwroot/images/products
var imagesProductsPath = Path.Combine(app.Environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
if (!Directory.Exists(imagesProductsPath))
{
  Directory.CreateDirectory(imagesProductsPath);
}

// Compatibilidad: si existen imágenes en la carpeta legacy wwwroot/Uploads, migrarlas a images/products
var legacyUploadsPath = Path.Combine(app.Environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
if (Directory.Exists(legacyUploadsPath))
{
  try
  {
    var files = Directory.GetFiles(legacyUploadsPath);
    foreach (var f in files)
    {
      var dest = Path.Combine(imagesProductsPath, Path.GetFileName(f));
      if (!File.Exists(dest))
      {
        File.Copy(f, dest);
      }
    }
  }
  catch (Exception ex)
  {
    var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger("Program");
    logger?.LogWarning(ex, "No se pudieron migrar archivos desde wwwroot/Uploads a wwwroot/images/products");
  }
}

// Servir estáticamente images/products en la ruta /images/products
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(imagesProductsPath),
  RequestPath = "/images/products"
});

// Mantener compatibilidad con la ruta legacy /Uploads para clientes antiguos (sirve archivos desde images/products también)
if (!Directory.Exists(legacyUploadsPath))
{
  // No crear la carpeta legacy para evitar duplicados, pero si no existe, mapear la misma ruta física
  // de todas formas para que /Uploads/* funcione redirigiendo a images/products
  // (no creamos, solo mapeamos al mismo proveedor)
}
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(imagesProductsPath),
  RequestPath = "/Uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
