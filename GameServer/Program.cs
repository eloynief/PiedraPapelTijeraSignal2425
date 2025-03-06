using GameServer.Hubs;

var builder = WebApplication.CreateBuilder(args);



// Agregar SignalR al contenedor de servicios
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage(); // Para obtener más detalles sobre el error.
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.MapHub<GameHub>("/salajuego");

app.MapRazorPages();







app.Run();
