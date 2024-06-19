using System.Text;
using DataShareData;
using DataShareData.Repository.AccountRepoFolder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebApplication1.Service.AccountService;
using WebApplication1.Service.FileStoreService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// AddEndpointsApiExplorer() registers services needed for API Explorer and endpoint routing.
builder.Services.AddEndpointsApiExplorer();

// AddSwaggerGen() registers services needed for Swagger/OpenAPI document generation.

// AddHttpContextAccessor() registers services to access the current HTTP context.
builder.Services.AddMvc();
builder.Services.AddHttpContextAccessor();

// AddControllers() registers services for MVC controllers and related features.
builder.Services.AddControllers();





//connect to db
// Add DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("Docker_Database")!));

var token = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Secret_Key").Value!);
// add and config jwt authen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            // Manual provide token
            ValidateIssuer = false,
            ValidateAudience = false,

            // Set sign value
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(token),
            //Set the allowable time difference between the server's clock and the token's clock
            ClockSkew = TimeSpan.Zero
        };

    });

// Add account service
// add scope -> onnce when request
// add singleton -> only one use for whole prj

builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IFileStoreRepo, FileStoreRepo>(); 
builder.Services.AddScoped<IFileStoreService, FileStoreService>();
builder.Services.AddScoped<ITextStoreRepo, TextStoreRepo>();
builder.Services.AddScoped<ITextStoreService, TextStoreService>();
// config CORS
builder.Services.AddCors(options => options.AddPolicy(name: "myOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5170")
            .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    }));

builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });
// build app
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseCors("NgOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

