using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;
using DynamicData;
using NewFileOrder.Models;
using NewFileOrder.Models.DbModels;
using NewFileOrder.ViewModels;
using NewFileOrder.Views;
using Microsoft.EntityFrameworkCore;

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
            
// source of knowledge: https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=visual-studio
            using (var db = new MyDbContext())
            {
                /*
                // Create
                Console.WriteLine("Insert");
                db.Files.Add(new FileModel { LastChecked = DateTime.Now, Hash = ";asdjfl;aj;", Path = "C:/nani", Name="nevim", FileTags = new List<FileTag>() });
                db.SaveChanges();

                // Read
                Console.WriteLine("Query");
                var file = db.Files
                    .OrderBy(b => b.Path)
                    .First();

                // Update
                Console.WriteLine("Update");
                file.Path = "C:/xd";
                file.FileTags.Add(
                    new FileTag {
                        File = file,
                        Tag = new TagModel
                        {
                            Name = "kendr",
                        } });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete");
                db.Remove(file);
                db.SaveChanges();
                
                // Create few test entries
                Console.WriteLine("Filling db");
                List<FileModel> files = new List<FileModel>();
                List<TagModel> tags = new List<TagModel>();

                tags.Add(new TagModel { Name = "a" });
                tags.Add(new TagModel { Name = "b" });
                tags.Add(new TagModel { Name = "c" });
                tags.Add(new TagModel { Name = "d" });
                tags.Add(new TagModel { Name = "e" });

                files.Add(new FileModel { Name = "kedr.txt", Path = "C:/Users/JanPro/Desktop/kys", Hash = "asjklhuTZU34567", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "chleba.docx", Path = "C:/Users/JanPro/Desktop/kys", Hash = "djwoiahdukhjJH", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "taveňák.cpp", Path = "C:/Users/JanPro/Desktop/kys", Hash = "3678zujGHJ", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "thinkpad.txt", Path = "C:/Users/JanPro/Desktop/kys", Hash = "!KLJFKSfxyfdsfsf", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "iduno", Path = "C:/Users/JanPro/Desktop/kys", Hash = "FESFffsdfsdfsdf", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "Manjaro Linux.iso", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "389ghugýžááš", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "Visual Studio.lnk", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "FDJKSHjghzuiujhf", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "vim", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "8347íuztuitu678", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "nevim", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "djhakdjkahskldh7", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "nemám potuchy", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "ada76468čščš34", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });
                files.Add(new FileModel { Name = "ahoj", Path = "C:/Users/JanPro/Desktop/jjjj", Hash = "Fdsfsfdsffff67", LastChecked = DateTime.Now, Metadata = "", IsMissing = false, FileTags = new List<FileTag>() });

                Random rnd = new Random();
                foreach (FileModel f in files)
                {
                    foreach (TagModel t in tags)
                        if (rnd.Next(2) == 1)
                            f.FileTags.Add(new FileTag { File = f, Tag = t });
                    db.Files.Add(f);
                }
                db.SaveChanges();

                /*
                var testFile = new FileModel { Name = "kedr.txt", Path = "C:/Users/JanPro/Desktop/kys", Hash = "asjklhuTZU34567", LastChecked = DateTime.Now, Metadata = "", IsMissing = false};
                var testTag = new TagModel { Name = "tagis" };

                testFile.FileTags = new List<FileTag>
                {
                    new FileTag
                    {
                        File = testFile,
                        Tag = testTag
                    }
                };

                db.Files.Add(testFile);
                db.SaveChanges();
                */
                /*
                var file = db.Files.Include(f => f.FileTags).First();

                file.FileTags.Add(new FileTag { File = file, Tag = db.Tags.First() });

                db.Files.Update(file);
                db.SaveChanges();
                */

                //var file = db.Files.Include(f => f.FileTags).First();
                //var tag = db.Tags.First();
                
                /*file.FileTags.Add(new FileTag
                {
                    File = file,
                    Tag = tag
                });*/
                db.SaveChanges();

               // var fileCheck = db.Files.Include(f => f.FileTags).Single(f => f.FileId == file.FileId);
                //var tagCheck = db.Tags.Include(t => t.FileTags).Single(t => t.TagId == tag.TagId);

                var window = new MainWindow
                {
                    DataContext = new MainWindowViewModel(db),
                };
                app.Run(window);
            }
        }
    }
}
