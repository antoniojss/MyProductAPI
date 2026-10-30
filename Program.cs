using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MyProductAPI.API.Filters;
using MyProductAPI.Application;
using MyProductAPI.Converters;
using MyProductAPI.Domain.Repositories.User;
using MyProductAPI.Exception.ExceptionBase;
using MyProductAPI.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Adiciona o seu conversor customizado corrigido
    options.JsonSerializerOptions.Converters.Add(new StringConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// teste de checkin 17-06-2026
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//1 - incluir o pacote do seagger no pacote nuget Swashbuckle.AspNetCore

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter only your access token. Swegger will add 'Bearer' prefix automatically",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(openApiDocument =>
    {
        return new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", openApiDocument),
                       // new List<string>() -- lista de string vazia
                       []
                    }

                };
    });
});


// 2. Injeção de Dependência (Requer as referências dos projetos Domain, Infra , Application e validator)

builder.Services.AddScoped<ProductValidator>();

// No Program.cs, basta adicionar essa linha e ela resolve tudo da infraestrutura de uma vez:
builder.Services.AddInfrastructure(builder.Configuration); 
builder.Services.AddApplication();


// 3. Configurações padrão da API e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



//5 - injetar o serviço de exception, para receber quaisquer erros de execão que retorrnar da API
builder.Services.AddMvc(options => options.Filters.Add<ExceptionFilter>());

//6 - converter todas as urls para minusculo, para não ter problemas de rotas com letras maiusculas e minusculas
//builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtoptions =>
    {
        var signingKey = builder.Configuration.GetValue<string>("Jwt:SigningKey")!;

        jwtoptions.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,

            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero
        };

        jwtoptions.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(subject, out var userId) == false)
                {
                    context.Fail("Invalid Token Subject");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadOnlyRepository>();

                var UserExists = await userRepository.ExistActiveUserWithId(userId);
                if (UserExists == false)
                {
                    context.Fail("User not found");
                    return;
                }
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = context.AuthenticateFailure switch
                {
                    null => new ResponseErrorJson(ResourceMessagesException.VALIDATION_ACCESS_TOKEN_REQUIRED),
                    SecurityTokenExpiredException => new ResponseErrorJson("Token Expired", accessTokenExpired: true),
                    _ => new ResponseErrorJson(ResourceMessagesException.VALIDATION_RESOURCE_ACCESS_DENIED),
                };

                await context.Response.WriteAsJsonAsync(response);
            }

        };

    });

var app = builder.Build();

// Configuração do pipeline HTTP (Swagger, Middlewares, etc.)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await ExecuteMigrations();

app.Run();


async Task ExecuteMigrations()
{
    await using var scope = app.Services.CreateAsyncScope();
    DatabaseMigration.ExecuteMigration(scope.ServiceProvider);
}

public partial class Program { } // Expose Program class for integration tests