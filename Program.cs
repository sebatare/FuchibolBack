using Fuchibol.ChatService.DataService;
using Fuchibol.ChatService.Hubs;
using Fuchibol.ChatService.MiddlewareExtensions;
using Fuchibol.ChatService.SubscribeTableDependencies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la app antes de construirla
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("reactNative", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://10.0.2.2:8081")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inicialización de la base de datos en memoria
builder.Services.AddSingleton<SharedDb>();
builder.Services.AddSingleton<UserHub>();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddSingleton<SubscribeUserTableDependency>();

// Configuración de JWT
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        Console.WriteLine("====================");
        Console.WriteLine("PARAMETROS DEL SERVICIO CORRECTO");
        Console.WriteLine("====================");
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                Console.WriteLine("accesToken:");
                Console.WriteLine(accessToken);
                Console.WriteLine("HttpContext");
                Console.WriteLine(context.HttpContext.Request.Path.StartsWithSegments("/touserhub"));
                Console.WriteLine("URL completa:");
                Console.WriteLine(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
                if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/touserhub"))
                {   Console.WriteLine("XXXXXXXXXXXXXXXXX");
                    context.Token = accessToken;
                }
                Console.WriteLine("Context Token desde la configuracion del token");
                Console.WriteLine(context.Token);
                Console.WriteLine("==========");
                Console.WriteLine(Task.CompletedTask);
                return Task.CompletedTask;
            }
        };
    });

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

app.UseAuthentication(); // Agrega UseAuthentication antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

// Mapea el hub de SignalR
app.MapHub<ChatHub>("/chat");
app.MapHub<UserHub>("/users");
app.MapHub<ToUserHub>("/touserhub");

app.UseUserTableDependecy();

app.Run();
