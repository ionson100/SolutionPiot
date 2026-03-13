using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using piotdll;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    // Настраиваем информацию об API (необязательно, но полезно)
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Your API Title",
        Version = "v1",
        Description = "Description of your API."
    });

    // --- ВАЖНАЯ ЧАСТЬ: Подключаем XML-комментарии ---
    // Получаем имя исполняемой сборки (вашего проекта)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // Строим полный путь к файлу. Файл будет лежать в той же папке, что и собранная DLL.
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "piotdll.xml");

    // Этот метод заставляет Swagger читать комментарии из XML
    options.IncludeXmlComments(xmlPath);

    // Второй параметр (true) включает комментарии для контроллеров, если вы их тоже добавите
    // options.IncludeXmlComments(xmlPath, true); 
});

// Настройка Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});






// Добавляем внешний файл конфигурации (будет смонтирован в Docker)
builder.Configuration.AddJsonFile("piot/settings.json", optional: true, reloadOnChange: true);

// Переменные окружения переопределяют значения из файлов
builder.Configuration.AddEnvironmentVariables();

// Регистрируем сервис
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));


builder.Services.AddHttpClient();






builder.Services.AddSwaggerGen();

var app = builder.Build();














// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try
{
    Log.Information("Запуск приложения");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось с ошибкой");
}
finally
{
    Log.CloseAndFlush();
}

//app.Run();