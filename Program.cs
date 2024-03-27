using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.CompanyBankAssetRepository;
using PayBridgeAPI.Repository.MainRepo;
using PayBridgeAPI.Repository.TransactionRepo;
using PayBridgeAPI.Repository.UserRepo;
using PayBridgeAPI.Services.AzureBlobs;
using PayBridgeAPI.Services.RESTServices;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetValue<string>("ApiSetting:Secret");

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    };
}
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. \\r\\n\\r\\n\" +\r\n  " +
        "      \"Enter 'Bearer' [space] and then your token in the text input.\\r\\n\\r\\n\" +\r\n " +
        "       \"Example: \\\"Bearer 12345abcdef\\\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddDbContext<PayBridgeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<PayBridgeDbContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton(u => new BlobServiceClient(builder.Configuration.GetConnectionString("ServiceClient")));
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IManagerRepository), typeof(ManagerRepository));
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IPersonalAccountRepository, PersonalAccountRepository>();
builder.Services.AddScoped<ICorporateAccountRepository, CorporateAccountRepository>();
builder.Services.AddScoped<IPersonalBankAccountRepository, PersonalBankAccountRepository>();
builder.Services.AddScoped<ICorporateBankAccountRepository, CorporateBankAccountRepository>();
builder.Services.AddScoped(typeof(ITranscationRepository<>), typeof(TransactionRepository<>));
builder.Services.AddScoped<IUserToUserTransactionRepository, UserToUserTransactionRepository>();
builder.Services.AddScoped<IBankCardRepository, BankCardRepository>();
builder.Services.AddScoped<ICompanyBankAssetRepository, CompanyBankAssetRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient<IBaseService, BaseService>();
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();
builder.Services.AddSingleton<IBaseService, BaseService>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
