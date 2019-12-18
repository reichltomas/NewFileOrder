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
            WatchRoots();
        }
        void UpdateFileModels(string path)
        {

        }
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
        /*void UpdateFilesInDatabase(List<FileModel> list)
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


        List<DirectoryModel> ListDirectoryDirectories(string path)
        {
            Directory.GetDirectories(path);
            return null;
        }
        string HashFile(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var hash = _hasher.ComputeHash(file);
                var hashString = Convert.ToBase64String(hash);
                if (hashString == "")
                    return Convert.ToBase64String(_hasher.ComputeHash(Encoding.UTF8.GetBytes(path)));
                ;
                return hashString;
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
            var roots = _db.Directories.Where(t => t.IsRoot).ToList();
            foreach (var root in roots)
            {
                var fsw = new FileSystemWatcher(FullPath(root));
                fsw.Changed += OnChanged;
                fsw.Created += OnCreated;
                fsw.Deleted += OnDeleted;
                fsw.Renamed += OnRenamed;

            }
        }
        //        WARNING UNTESTED CODE, MIGHT BREAK EVERYTHING
        private void OnRenamed(object sender, RenamedEventArgs e)
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

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var name = e.Name;
            //needs testing
            var path = e.FullPath.Substring(0, e.FullPath.Length - name.Length);
            var file = _db.Files.Where(b => b.Name == name).Where(a => a.IsMissing == false).Where(c => c.Path == path).First();
            //DeleteFileFromDB(file);
            file.IsMissing = true;
            UpdateFileInDatabase(file);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            var name = e.Name;
            var file = _db.Files.Where(a => a.IsMissing == true).Where(b => b.Hash == HashFile(path)).First();
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

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var name = e.Name;
            var path = e.FullPath.Substring(0, e.FullPath.Length - name.Length);
            var file = _db.Files.Where(b => b.Name == name).Where(a => a.IsMissing == false).Where(c => c.Path == path).First();
            file.Hash = HashFile(e.FullPath);
            file.LastChecked = DateTime.Now;
        }


        public List<FileModel> GetFilesWithTags(ICollection<TagModel> tags)
        {
            return _db.Files.Where(file => file.FileTags.All(filetag => tags.Contains(filetag.Tag))).ToList();
        }
    }
}

