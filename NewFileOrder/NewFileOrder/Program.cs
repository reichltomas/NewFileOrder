using System;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;
using DynamicData;
using NewFileOrder.Models;
using NewFileOrder.Models.DbModels;
using NewFileOrder.ViewModels;
using NewFileOrder.Views;

namespace NewFileOrder
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var window = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
// source of knowledge: https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=visual-studio
            using (var db = new MyDbContext())
            {
                // Create
                Console.WriteLine("Insert");
                db.Files.Add(new FileModel { LastChecked = DateTime.Now, Hash = ";asdjfl;aj;", Path = "C:/nani" });
                db.SaveChanges();

                // Read
                Console.WriteLine("Query");
                var file = db.Files
                    .OrderBy(b => b.Path)
                    .First();

                // Update
                Console.WriteLine("Update");
                file.Path = "C:/xd";
                file.Tags.Add(
                    new TagModel
                    {
                        Name = "kendr",
                    });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete");
                db.Remove(file);
                db.SaveChanges();
            }
            app.Run(window);
        }
    }
}
