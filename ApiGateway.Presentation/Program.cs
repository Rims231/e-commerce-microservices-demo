using ApiGateway.Presentation.Middleware;
using eCommerce.SharedLibrary.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot json file
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication from SharedLibrary
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

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

// Attach Api-Gateway signature to all requests
app.UseMiddleware<AttachSignatureToRequest>();

// Use Ocelot middleware
await app.UseOcelot();

// Use Ocelot middleware
await app.UseOcelot();

app.MapControllers();

app.Run();