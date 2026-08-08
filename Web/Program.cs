using Application.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString(
        "GestionHoteliereConnection")
    ?? throw new InvalidOperationException(
        "La chaîne de connexion GestionHoteliereConnection est introuvable.");

builder.Services.AddDbContext<GestionHoteliereDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ITypeChambreRepository), typeof(TypeChambreRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IChambreRepository), typeof(ChambreRepository));
builder.Services.AddScoped(typeof(IReservationRepository), typeof(ReservationRepository));
builder.Services.AddScoped(typeof(IFactureRepository), typeof(FactureRepository));
builder.Services.AddScoped(typeof(ISejourRepository), typeof(SejourRepository));
builder.Services.AddScoped(typeof(IPaiementRepository), typeof(PaiementRepository));
builder.Services.AddScoped(typeof(IClientRepository), typeof(ClientRepository));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
