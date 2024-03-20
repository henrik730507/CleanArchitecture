using System.Text;
using CityInfo.API;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddSwaggerGen();

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
