using Serilog;

namespace Serilog_Tutorial
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = "Server=(local);Database=Development;Trusted_Connection=True;Encrypt=False";
            var sinkOptions = new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
            {
                TableName = "Serilog",
                AutoCreateSqlTable = true,
            };
            var columnOptions = new Serilog.Sinks.MSSqlServer.ColumnOptions();

            columnOptions.Store.Add(Serilog.Sinks.MSSqlServer.StandardColumn.LogEvent);
            columnOptions.Store.Remove(Serilog.Sinks.MSSqlServer.StandardColumn.MessageTemplate);
            columnOptions.Properties.DataLength = 1000;
            columnOptions.Id.DataType = System.Data.SqlDbType.BigInt;

            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                 .WriteTo.MSSqlServer(connectionString: connectionString, sinkOptions: sinkOptions, columnOptions: columnOptions)
                 .MinimumLevel.Information()
                 .CreateLogger();

            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
