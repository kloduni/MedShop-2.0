using MedShop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddMedShopDbContext(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("MedShopAppOnly", opt =>
    {
        opt.WithOrigins("https://localhost:7209")
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MedShopAppOnly");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
