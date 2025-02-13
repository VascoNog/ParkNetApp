
using ParkNetApp;
using ParkNetApp.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ParkNetConnection") ?? throw new InvalidOperationException("Connection string 'ParkNetConnection' not found.");
builder.Services.AddDbContext<ParkNetDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ParkNetDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddScoped<ParkNetRepository>();


var app = builder.Build();


//string fileName = "Schema_01LisbonPortugal.txt";
//string schemaPath = Utilities.GetParkingLotSchemaPath(fileName);

//string stringSchema = @"
//MM      CCCC
//MM MMM  MMMM
//CCCCCCCCCCCC

//MMMM MMM
//MMMM MMM
//MMMM MMM";

//Utilities.SaveParkingLotSchema(schemaPath,stringSchema);





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
