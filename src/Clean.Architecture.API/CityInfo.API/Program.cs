using System.Reflection;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfo.API;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add support for xml. Consumers can now also get the response in xml if they sets the Accept header to application/xml
builder.Services.AddControllers(options =>
{
  options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
  .AddXmlDataContractSerializerFormatters();

//Extend the problemDetails that consumers get with a response that indicates a problem.
builder.Services.AddProblemDetails(options =>
{
  options.CustomizeProblemDetails = ctx =>
  {
    ctx.ProblemDetails.Extensions.Add("AdditionalInfo", "Additional info example");
    ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
  };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(setupAction =>
{
  var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
  setupAction.IncludeXmlComments(xmlCommentsFullPath);

  setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
  {
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    Description = "Input a valid token to access this API"
  });

  setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth" }
            }, new List<string>() }
    });
});

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new()
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"] ?? ""))
      };
    }
    );

builder.Services.AddApiVersioning(setupAction =>
{
  setupAction.AssumeDefaultVersionWhenUnspecified = true;
  setupAction.DefaultApiVersion = new ApiVersion(1, 0);
  setupAction.ReportApiVersions = true;
}).AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  _ = endpoints.MapControllers();
});

app.Run();
