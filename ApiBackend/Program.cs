
var builder = WebApplication.CreateBuilder(args);

var enCorsPolicy = "enCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(enCorsPolicy, builder => {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(enCorsPolicy);
}
else
{
    app.UseHttpsRedirection();
    app.UseAuthorization();
    //app.UseCors(prodCorsPolicy);
}

app.MapControllers();

app.Run();
