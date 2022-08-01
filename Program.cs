// add needed imports 
using GroupMealApi.Services;
using Microsoft.AspNetCore.HttpLogging;
using GroupMealApi.AppHubs;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add singleton services to the container.
builder.Services
.AddSingleton<GroupService>()
.AddSingleton<AccountService>()
.AddSingleton<MealChoiceService>()
.AddSingleton<ElectionService>()
.AddSingleton<ElectionRoundService>();


// Controllers
builder.Services.AddControllers()
.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddSignalR();


// Controller documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// background services


// Configure logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddAWSProvider();


builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});


// Add the cors configuration for the signllr hub
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
    });
});


var app = builder.Build();


// Group Hub
app.MapHub<GroupHub>("/group");
app.MapHub<ElectionRoundHub>("/electionround");


// Configure dev environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpLogging();
    app.UseDeveloperExceptionPage();
    app.UseCertificateForwarding();
}

// Configure production environment.
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}



app.UseCors("ClientPermission");
app.UseHttpLogging();
app.UseRouting();
app.MapControllers();
app.Run();

