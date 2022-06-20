using API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//----------------------------------------------------
////injecting database services
//builder.Services.AddSqlDatabaseConnector();

////injecting producer services
//builder.Services.AddProducerConnector();

////injecting internal services
//builder.Services.AddInternalServices();

builder.Services.AddServices();
//----------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//ConsumerApp.StartApp myapp = new ConsumerApp.StartApp();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
