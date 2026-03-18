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
        Title = "API ТС ПИоТ",
        Version = "v2",
        Description = "Проверка кода маркировки."
    });
    // --- ВАЖНАЯ ЧАСТЬ: Подключаем XML-комментарии ---
    // Получаем имя исполняемой сборки (вашего проекта)
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // Строим полный путь к файлу. Файл будет лежать в той же папке, что и собранная DLL.
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "piotdll.xml");

    // Этот метод заставляет Swagger читать комментарии из XML
    options.IncludeXmlComments(xmlPath,true);
    // Второй параметр (true) включает комментарии для контроллеров, если вы их тоже добавите
});

// 1. Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Указываем вывод в консоль
    .WriteTo.File("piot/logs/log-.txt",
        rollingInterval: RollingInterval.Day, // Ротация каждый день (создаст log-20231005.txt)
        rollOnFileSizeLimit: true,           // Создавать новый файл при достижении лимита размера
        fileSizeLimitBytes: 10 * 1024 * 1024, // Лимит 10 МБ (по умолчанию 1 ГБ)
        retainedFileCountLimit: 7            // Хранить только последние 7 файлов
    )
    .CreateLogger();

builder.Host.UseSerilog();
// Настройка Serilog
//builder.Host.UseSerilog((context, config) =>
//{
//    config.ReadFrom.Configuration(context.Configuration);
//});


// Добавляем внешний файл конфигурации (будет смонтирован в Docker если вы не передадите тома в параметрах запуска)
builder.Configuration.AddJsonFile("piot/settings.json", optional: true, reloadOnChange: true);

// Переменные окружения переопределяют значения из файлов
builder.Configuration.AddEnvironmentVariables();

// Регистрируем сервис
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));

//builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles(); // ищет index.html, default.html и т.д.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseAuthorization();

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