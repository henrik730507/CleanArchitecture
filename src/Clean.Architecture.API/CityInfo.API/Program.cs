using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add support for xml. Consumers can now also get the response in xml if they sets the Accept header to application/xml
builder.Services.AddControllers(options =>
{
  options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  _ = endpoints.MapControllers();
});

app.Run();
