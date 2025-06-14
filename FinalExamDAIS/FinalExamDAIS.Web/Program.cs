using FinalExamDAIS.Repository;
using FinalExamDAIS.Repository.Implementations.Account;
using FinalExamDAIS.Repository.Implementations.User;
using FinalExamDAIS.Repository.Interfaces.User;
using FinalExamDAIS.Services.Implementations.Account;
using FinalExamDAIS.Services.Implementations.Authentication;
using FinalExamDAIS.Services.Interfaces.Account;
using FinalExamDAIS.Services.Interfaces.Authentication;
using FinalExamDAIS.Repository.Interfaces.Account;
using FinalExamDAIS.Repository.Interfaces.Payment;
using FinalExamDAIS.Repository.Implementations.Payment;
using FinalExamDAIS.Services.Interfaces.Payment;
using FinalExamDAIS.Services.Implementations.Payment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register connection string for dependency injection
builder.Services.AddSingleton(connectionString);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
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
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
