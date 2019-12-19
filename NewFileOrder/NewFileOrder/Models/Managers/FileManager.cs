using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NewFileOrder.Models.Managers
{
    class FileManager : Manager
    {
        private readonly SHA256 _hasher = SHA256.Create();
        private List<FileSystemWatcher> _fileWatchers;


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
            Cleanup();
            WatchRoots();
        }
        public void AddRoot(string path)
        {
            var hash = HashDirectory(path);
            path = path.Replace('\\', '/');
            var name = path.Split('/').Last();
            //-1 to not include /
            var pth = path.Substring(0, path.Length - name.Length - 1);
            //Console.WriteLine()
            var files = ListFiles(path);
            PutFilesInDatabase(files);
            var dir = new DirectoryModel { IsRoot = true, Path = pth, Name = name, Hash = HashDirectory(path), };
            PutDirectoryInDB(dir);
            WatchRoots();
        }

        private void PutDirectoryInDB(DirectoryModel dir)
        {
            _db.Directories.Add(dir);
            _db.SaveChanges();
        }

        void UpdateFileModels(string path)
        {

        }
        //ez and dumb
        List<FileModel> ListFiles(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                                 .ToList();

            List<FileModel> filesList = new List<FileModel>();
            foreach (var filepath in files)
            {
                //filepath = filepath.Replace(@"\\", "/");
                var name = filepath.Replace("\\", "/").Split('/').Last();
                var fpath = filepath.Substring(0, filepath.Length - name.Length - 1);
                var hash = HashFile(filepath);
                filesList.Add(new FileModel { LastChecked = DateTime.Now, Name = name, Path = fpath, Hash = hash });
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
        void PutFilesInDatabase(List<FileModel> list)
        {
            _db.Files.AddRange(list);
            _db.SaveChanges();
        }
        void UpdateFileInDatabase(FileModel file)
        {
            _db.Files.Update(file);
            _db.SaveChanges();
        }

        //TODO check files when restarted
        /*void CheckAfterRestart(List<FileModel> list)
        {
            foreach (FileModel file in list)
            {
                if (!File.Exists(FullPath(file)))
                {
                    file.IsMissing = true;
                }
                else
                {

                    file.Hash = HashFile(FullPath(file));
                }
                // if soubor neexistuje
                //      mark as moved
                // else soubor existuje
                //      if hash souboru != hash z db (= file.Hash)
                //          ulož do databáze nový hash
            } // TODO: renaming
            _db.Files.UpdateRange(list);
        }*/

        //todo někdy recursively
        List<DirectoryModel> ListDirectoryDirectories(string path)
        {
            Directory.GetDirectories(path);

            return null;
        }
        string HashFile(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                var hash = Convert.ToBase64String(_hasher.ComputeHash(file));//+Convert.ToBase64String(_hasher.ComputeHash(Encoding.UTF8.GetBytes(path)));
                                                                             // if (hash== "")
                                                                             //    return Convert.ToBase64String(_hasher.ComputeHash(Encoding.UTF8.GetBytes(path)));

                return hash;
            }
        }

        public string HashDirectory(string path)
        {
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
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    _hasher.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    _hasher.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(_hasher.Hash);
        }

        private void WatchRoots()
        {
            if (_fileWatchers != null)
            {
                foreach (var f in _fileWatchers)
                    f.Dispose();
            }
            var roots = _db.Directories.Where(t => t.IsRoot == true).ToList();
            foreach (var root in roots)
            {
                var fsw = new FileSystemWatcher(FullPath(root));
                fsw.EnableRaisingEvents = true;
                fsw.Changed += new FileSystemEventHandler(OnChanged);
                fsw.Created += new FileSystemEventHandler(OnCreated);
                fsw.Deleted += new FileSystemEventHandler(OnDeleted);
                fsw.Renamed += new RenamedEventHandler(OnRenamed);

            }
        }
        //        WARNING UNTESTED CODE, MIGHT BREAK EVERYTHING 
        public void OnRenamed(object sender, RenamedEventArgs e)
        {
            var oname = e.OldName;
            var nname = e.Name;
            var path = e.FullPath;
            //possible problem with empty files... 
            var file = _db.Files.Where(b => b.Hash == HashFile(path)).Where(a => a.IsMissing == false).First();
            file.Name = nname;
            file.LastChecked = DateTime.Now;
            UpdateFileInDatabase(file);
        }

        public void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var name = e.Name;
            //needs testing
            var path = e.FullPath.Substring(0, e.FullPath.Length - name.Length).Trim('\\');
            var file = _db.Files.Where(b => b.Name == name).Where(a => a.IsMissing == false).Where(c => c.Path == path).FirstOrDefault();
            //DeleteFileFromDB(file);
            file.IsMissing = true;
            UpdateFileInDatabase(file);
        }

        public void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory))
                return; //ignore directories, only process files
            var path = e.FullPath;
            var name = e.Name;
            FileModel file = _db.Files.Where(a => a.IsMissing == true).Where(b => b.Hash == HashFile(path)).FirstOrDefault();
            if (file == null)
            {
                file = new FileModel { Hash = HashFile(path), Name = name, Path = path, LastChecked = DateTime.Now };
                PutFileInDB(file);
            }
            else
            {
                file.IsMissing = false;
                file.LastChecked = DateTime.Now;
                UpdateFileInDatabase(file);
            }
        }

        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            var name = e.Name;
            var path = e.FullPath.Substring(0, e.FullPath.Length - name.Length).Trim('\\');
            var file = _db.Files.Where(b => b.Name == name).Where(a => a.IsMissing == false).Where(c => c.Path == path).FirstOrDefault();
            if (file != null)
            {
                file.Hash = HashFile(e.FullPath);
                file.LastChecked = DateTime.Now;
                PutFileInDB(file);
            }
        }


        public List<FileModel> GetFilesWithTags(ICollection<TagModel> tags)
        {
            return _db.Files.Where(file => file.FileTags.All(filetag => tags.Contains(filetag.Tag))).ToList();
        }
        private void Cleanup()
        {
            if (_fileWatchers == null)
                goto kys;
            foreach (var f in _fileWatchers)
            {
                f.Dispose();
            }
            kys:
            _db.Files.RemoveRange(_db.Files);
            _db.Directories.RemoveRange(_db.Directories);
            _db.FileTags.RemoveRange(_db.FileTags);
            _db.SaveChanges();
        }
    }
}

