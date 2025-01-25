using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prog.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            LifetimeValidator = CustomLifeTimeValidator,
            IssuerSigningKey = AuthOptions.GetKey(),
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddSingleton<IDBManager, DBManager>();
builder.Services.AddSingleton<IAuthJwt, AuthJwt>();    

var app = builder.Build();

const string PATH_USERS = "/home/ian/Desktop/FinalCourseProject/Server/users.db";

var dbManager = app.Services.GetRequiredService<IDBManager>();
if (!dbManager.ConnectToDB(PATH_USERS))
{
    Console.WriteLine("Failed connection to users database: " + PATH_USERS);
    Console.WriteLine("Shutdown");
    return;
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

dbManager.Disconnect();

bool CustomLifeTimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
{
    if (expires == null)
        return false;
    return expires > DateTime.UtcNow;
}