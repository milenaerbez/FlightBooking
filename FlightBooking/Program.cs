using FlightBooking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using FlightBooking.Hub;
using FlightBooking;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("FlightApp")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSignalR();

var app = builder.Build();

//static IHostBuilder CreateHostBuilder(string[] args) =>
//   Host.CreateDefaultBuilder(args)
//       .ConfigureWebHostDefaults(webBuilder =>
//       {
//           webBuilder.ConfigureServices((context, services) =>
//           {

//               services.AddSignalR();
//           });
//           webBuilder.Configure((context, app) =>
//           {
//               if (context.HostingEnvironment.IsDevelopment())
//               {
//                   app.UseDeveloperExceptionPage();
//               }
//               else
//               {
//                   app.UseExceptionHandler("/Home/Error");
//                   app.UseHsts();
//               }

//               app.UseHttpsRedirection();
//               app.UseStaticFiles();


//               app.UseEndpoints(endpoints =>
//               {
//                   endpoints.MapHub<ReservationHub>("/ReservationHub");
//                   endpoints.MapControllerRoute(
//                       name: "default",
//                       pattern: "{controller=Home}/{action=Index}/{id?}");
//               });
//           });
//       });



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ReservationHub>("/ReservationHub");
});
app.Run();
