﻿using Microsoft.EntityFrameworkCore;
using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewFileOrder.Models.Managers
{
    public class FileManager : Manager
    {
        private const string HASH_OF_EMPTY_FILE = "lasdl;fj;lajdlfadfjladsjf;ajldfj;adjfla;d;;adslfja";
        private readonly SHA256 _hasher = SHA256.Create();
        private int _watchTime = 5000;
        public event EventHandler<NewFileEventArgs> NewFilesEvent;

        protected virtual void OnNewFiles(NewFileEventArgs e)
        {
            EventHandler<NewFileEventArgs> handler = NewFilesEvent;
            handler?.Invoke(this, e);
        }


        private string FullPath(IFileSystemEntry model)
        {
            return model.Path + "/" + model.Name;
        }
        void PutFileInDB(FileModel file)
        {
            _db.Files.Add(file);
            _db.SaveChanges();
        }
        void DeleteFileFromDB(FileModel file) { _db.Files.Remove(file); _db.SaveChanges(); }

        public FileManager(MyDbContext dbContext) : base(dbContext)
        {
            //Cleanup();
            Task t = new Task(async () =>
            {
                while (true)
                {
                    Thread.Sleep(_watchTime);
                    try
                    {
                        await Watch();
                    }
                    catch (InvalidOperationException)
                    {
                        // if we have database collisions then dont check files that often
                        _watchTime += 1000;
                    }
                }
            });
            t.Start();
        }
        //todo get rid of this method
        public async void AddRoot(string path)
        {
            var hash = HashDirectory(path);
            path = path.Replace("\\", "/");
            var name = path.Split('/').Last();
            //-1 to not include /
            var pth = path.Substring(0, path.Length - name.Length - 1);
            var dir = new DirectoryModel { IsRoot = true, Path = pth, Name = name, Hash = hash, };
            await PutDirectoryInDB(dir);
            var files = ListFiles(path);
            await PutFilesInDB(files);

        }
        public void AddRootIfNotInDb(string path)
        {
            path = path.Replace("\\", "/");
            var name = path.Split('/').Last();
            //-1 to not include /
            var pth = path.Substring(0, path.Length - name.Length - 1);
            if (_db.Directories.Where(d => d.Path == pth).Where(d => d.Name == name).Count() == 0)
                AddRoot(path);
        }

        public async void RemoveDirectory(DirectoryModel dir)
        {
            _db.Directories.Remove(dir);
            await _db.SaveChangesAsync();
        }

        private async Task PutDirectoryInDB(DirectoryModel dir)
        {
            _db.Directories.Add(dir);
            await _db.SaveChangesAsync();
        }
        //ez and dumb
        List<FileModel> ListFiles(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                                 .ToList();

            List<FileModel> filesList = new List<FileModel>();
            foreach (var filepath in files)
            {
                bool isHidden = (File.GetAttributes(filepath) & FileAttributes.Hidden) == FileAttributes.Hidden;
                if (isHidden)
                { continue; }
                var fp = filepath.Replace("\\", "/");
                var name = fp.Split('/').Last();
                var fpath = fp.Substring(0, fp.Length - name.Length - 1);
                var hash = HashFile(filepath);
                filesList.Add(new FileModel
                {
                    Created = File.GetCreationTime(filepath),
                    LastChecked = DateTime.Now,
                    Name = name,
                    Path = fpath,
                    Hash = hash
                });
                //TODO code for pseudotag
            }
            return filesList;
        }
        //hard way with directories
        List<FileModel> ListDirectoryFiles(string path)
        {
            List<FileModel> filesList = new List<FileModel>();
            var files = Directory.GetFiles(path);
            foreach (var filepath in files)
            {
                var name = filepath.Substring(path.Length + 1);
                var hash = HashFile(filepath);
                filesList.Add(new FileModel { LastChecked = DateTime.Now, Name = name, Path = path, Hash = hash });
                //TODO code for pseudotag
            }
            return filesList;
        }
        async Task PutFilesInDB(List<FileModel> list)
        {
            _db.Files.AddRange(list);
            await _db.SaveChangesAsync();
            OnNewFiles(new NewFileEventArgs { Files = list });
        }
        public async void UpdateFileInDB(FileModel file)
        {
            _db.Files.Update(file);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateFilesInDB(List<FileModel> files)
        {
            _db.Files.UpdateRange(files);
            await _db.SaveChangesAsync();
        }

        //todo někdy recursively
        List<DirectoryModel> ListDirectoryDirectories(string path)
        {
            Directory.GetDirectories(path);

            return null;
        }
        string HashFile(string path)
        {
            if (new FileInfo(path).Length == 0)
            {
                return HASH_OF_EMPTY_FILE;
            }
            try
            {
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    return Convert.ToBase64String(_hasher.ComputeHash(file));

                }
            }
            catch (System.IO.IOException)
            {/*file is open or something*/
                return HASH_OF_EMPTY_FILE;
            }
        }

        public string HashDirectory(string path)
        {
            return "zjistili jsme ze tohle neni dulezite";
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();


            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath);
                _hasher.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                try
                {
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (i == files.Count - 1)
                        _hasher.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    else
                        _hasher.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
                catch (System.IO.IOException) {/*file is open or something*/}

            }
            try
            {
                return BitConverter.ToString(_hasher.Hash);
            }
            catch { return "kednsadlfa;lsdf;ja"; }
        }
        //TODO rewrite this method from scratch knowing that DateCreated is almost unique identifier and who cares about weird generated files
        private async Task Watch()
        {
            var realFiles = new List<FileModel>();
            var roots = await _db.Directories.Where(t => t.IsRoot == true).ToListAsync();
            foreach (var root in roots)
            {
                realFiles.AddRange(ListFiles(FullPath(root)));
                //check things recursively
            }
            var dbFiles = await _db.Files.ToListAsync();
            var newFiles = new List<FileModel>();
            foreach (var realFile in realFiles)
            {
                realFile.Path = realFile.Path.Replace("\\", "/");
                //nothing happened or file was found
                var dbFile = dbFiles.Where(a => a.Name == realFile.Name).Where(b => b.Path == realFile.Path).Where(c => c.Hash == realFile.Hash).FirstOrDefault();
                if (dbFile != null)
                {
                    dbFile.IsMissing = false;
                    dbFile.LastChecked = realFile.LastChecked;
                    continue;
                }
                //rename in same directory,todo validate emptyfile
                dbFile = dbFiles.Where(a => a.Name != realFile.Name).Where(b => b.Path == realFile.Path
).Where(c => c.Hash == realFile.Hash).Where(d => d.Created == realFile.Created).FirstOrDefault();
                if (dbFile != null)
                {
                    dbFile.Name = realFile.Name;
                    dbFile.IsMissing = false;
                    dbFile.LastChecked = realFile.LastChecked;
                    continue;
                }
                //move, todo same files in 2 directories
                dbFile = dbFiles.Where(a => a.Name == realFile.Name).Where(b => b.Path != realFile.Path).Where(x => x.Created == realFile.Created).Where(c => c.Hash == realFile.Hash).FirstOrDefault();
                if (dbFile != null)
                {
                    dbFile.Path = realFile.Path;
                    dbFile.IsMissing = false;
                    dbFile.LastChecked = realFile.LastChecked;
                    continue;
                }
                //changed
                dbFile = dbFiles.Where(a => a.Name == realFile.Name).Where(b => b.Path == realFile.Path).Where(c => c.Created == realFile.Created).FirstOrDefault();
                if (dbFile != null)
                {
                    dbFile.Path = realFile.Path;
                    dbFile.Hash = realFile.Hash;
                    dbFile.IsMissing = false;
                    dbFile.LastChecked = realFile.LastChecked;
                    continue;
                }
                newFiles.Add(realFile);

            }
            //when we dont see a file for a while we mark it as missing 
            //Todo delete files that have been missing for a long time
            foreach (var f in dbFiles.Where(a => a.LastChecked < DateTime.Now.AddSeconds(-30)))
            {
                f.IsMissing = true;
            }
            await UpdateFilesInDB(dbFiles);

            if (newFiles.Count > 0)
            {
                await PutFilesInDB(newFiles);
            }

        }


        public List<FileModel> GetFilesWithTags(ICollection<TagModel> tags, bool includeMissing = false)
        {
            HashSet<FileModel> files = new HashSet<FileModel>(GetFilesFromFileTags(tags.First().FileTags).Where(a => a.IsMissing == false));

            foreach (var tag in tags.Skip(1))
            {
                files.IntersectWith(GetFilesFromFileTags(tag.FileTags));
            }
            if (files.Count == 0)
                throw new Exception("Nenalezen soubor, který by obsahoval všechny tagy");

            if (includeMissing)
                return Enumerable.OrderBy(files, x => x.Name).ToList();
            else
                return Enumerable.OrderBy(files.Where(f => !f.IsMissing), x => x.Name).ToList();

        }

        public List<FileModel> GetFilesFromFileTags(ICollection<FileTag> fileTags)
        {
            List<FileModel> files = new List<FileModel>();

            foreach (FileTag ft in fileTags)
                files.Add(ft.File);
            return files;
        }

        public List<FileModel> GetAllFiles(bool includeMissing = false, bool includeFileTags = false)
        {
            IQueryable<FileModel> dbTable;
            if (includeFileTags)
                dbTable = _db.Files.Include(f => f.FileTags);
            else
                dbTable = _db.Files;

            if(includeMissing)
                return Enumerable.OrderBy(dbTable, x => x.Name).ToList();
            else
                return Enumerable.OrderBy(dbTable.Where(f => !f.IsMissing), x => x.Name).ToList();
        }

        public List<DirectoryModel> GetAllDirectories() => Enumerable.OrderBy(_db.Directories, x => x.Name).ToList();

        async private void Cleanup()
        {
            try
            {
                _db.Files.RemoveRange(_db.Files);
                _db.Directories.RemoveRange(_db.Directories);
                _db.FileTags.RemoveRange(_db.FileTags);
                await _db.SaveChangesAsync();
            }
            catch { }
        }

        public void InitialDirectory()
        {
            if(_db.Directories.Count() == 0)
            {
                var dir = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/nfo_files");
                AddRoot(dir.FullName);
            }
        }
    }
}

