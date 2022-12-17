using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services;
using Blog_MVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// custom services
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));

// API google auth
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? Environment.GetEnvironmentVariable("ClientId")!;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("ClientSecret")!;
});

builder.Services.AddMvc();

// Add Swashbuckle for API display formatting
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Alex Blog API",
        Version = "v1",
        Description = "Serve up Blog Data using .Net 6 APIs",
        Contact = new OpenApiContact
        {
            Name = "A.Yang",
            Email = "AYangDevv@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/alexyanglinked/")
        }
    });
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

// allows API to be accessed
builder.Services.AddCors(obj =>
{
    obj.AddPolicy("DefaultPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// applies API policy
app.UseCors("DefaultPolicy");

// calls ManageData method to manipulate database, scope creates an IServiceProvider
var scope = app.Services.CreateScope();
await DataUtility.ManageDataAsync(scope.ServiceProvider);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// more Swashbuckle API
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "PublicAPI v1");
    s.InjectStylesheet("/css/swagger.css");
    s.InjectJavascript("/js/swagger.js");

    s.DocumentTitle = "Alex Blog API";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "custom",
    pattern: "Content/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
