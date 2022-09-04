using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScheduleApi.Data;
using ScheduleApi.Extensions;
using ScheduleApi.Filters;
using ScheduleApi.Services.AuthService;
using ScheduleApi.Services.EmployeeService;
using ScheduleApi.Services.RequestService;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Description = "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token} \"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.SchemaFilter<SwaggerSchemaFilter>();
});

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options => {
    options.AddPolicy(myAllowSpecificOrigins, policy => {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRequestService, RequestService>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.Authority = builder.Configuration["Auth0:Domain"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        //options.TokenValidationParameters = new TokenValidationParameters {

        //    ValidateIssuer = true,
        //    ValidateAudience = true,
        //    ValidateLifetime = true,
        //    ValidateIssuerSigningKey = true,
        //    ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //    ValidAudience = builder.Configuration["Jwt:Audience"],
        //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        //};
        //options.Events = new JwtBearerEvents();
        //options.Events.OnTokenValidated = async (context) => {
        //    string ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
        //    IAuthService authService = context.Request.HttpContext.RequestServices.GetService<IAuthService>();
        //    JwtSecurityToken jwtToken = context.SecurityToken as JwtSecurityToken;
        //    if (!await authService.IsTokenIpAddressValid(jwtToken.RawData, ipAddress)) {
        //        context.Fail("Invalid Token Details");
        //    }
        //};
    });
Console.WriteLine("CONNECTION_STRING: " + builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDbContext<ScheduleDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors(myAllowSpecificOrigins);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
