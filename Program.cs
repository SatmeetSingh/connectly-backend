using dating_app_backend.src.DB;
using dating_app_backend.src.Service;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
// Add services to the container.

builder.Services.AddControllers();

builder.Logging.ClearProviders(); // Optionally clear the default providers
builder.Logging.AddConsole(); // Add the Console Logger
builder.Logging.AddDebug(); // Add the Debug Logger
builder.Logging.AddEventSourceLogger(); // Add Event Source Logger (Windows-only)

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<LikesService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<FollowService>();
builder.Services.AddScoped<FileService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>    
    {
        builder.AllowAnyOrigin() 
               .AllowAnyHeader()  
               .AllowAnyMethod(); 
    });
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue; // Allow files up to 2 GB
});

builder.Services.AddRateLimiter(option =>
{
    option.AddFixedWindowLimiter("Fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

builder.Services.AddSwaggerGen(c =>     
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Connectly(Social Media Api)", Version = "v1" });
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload/images")),
    RequestPath = "/Upload/images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload/Posts")),
    RequestPath = "/Upload/Posts"
});

app.UseWebSockets();        // Enable WebSockets
app.UseRouting();

app.UseCors("AllowAll");          // Always b/w Routing and Authoriztion


app.UseAuthorization();
app.UseRateLimiter();             
//app.UseAuthentication();

app.UseHttpsRedirection();
app.MapControllers();


app.MapHub<MessageHub>("/ChatHub");



app.Run();
