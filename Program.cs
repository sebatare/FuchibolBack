using Fuchibol.ChatService.DataService;
using Fuchibol.ChatService.Hubs;
using Fuchibol.ChatService.MiddlewareExtensions;
using Fuchibol.ChatService.SubscribeTableDependencies;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la app antes de construirla
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("reactNative", builder =>
    {
        builder.WithOrigins("http://10.0.2.2:8081")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inicialicacion de mi base de datos en memoria
builder.Services.AddSingleton<SharedDb>();
builder.Services.AddSingleton<UserHub>();
builder.Services.AddSingleton<SubscribeUserTableDependency>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("reactNative"); // Asegúrate de que CORS esté configurado antes de Authorization

app.UseAuthorization();

app.MapControllers();

// Mapea el hub de SignalR
app.MapHub<ChatHub>("/chat");
app.MapHub<UserHub>("/users");
app.MapHub<ToUserHub>("/touserhub");

app.UseUserTableDependecy();

app.Run();
