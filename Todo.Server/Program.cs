using TodoServer.Extensions;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AllowCors(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddDependencInjection(builder.Configuration);

var app = builder.Build();

app.UseCors("DynamicCorsPolicy");

// Initialize database
await app.Services.InitializeDatabaseAsync();

app.UseDefaultFiles();
app.UseStaticFiles();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
