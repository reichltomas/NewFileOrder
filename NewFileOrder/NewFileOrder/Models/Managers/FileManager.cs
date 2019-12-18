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
        private SHA256 _hasher = SHA256.Create();
        // private List<FileWatcher> ;

        private string FullPath(FileModel fm)
        {
            return fm.Path + "/"+ fm.Name;
        }
        void PutFileInDB(FileModel file)
        {
            _db.Files.Add(file);
            _db.SaveChanges();
        }

        public FileManager(MyDbContext dbContext):base(dbContext)
        {

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
        }        void UpdateFilesInDatabase(List<FileModel> list)
        {
            foreach(FileModel file in list)
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
        }


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
                    return "2408585c623a422acb2ee2995a622329655c48d2b745d55bf1fa92ebb6e6244d";
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


        public List<FileModel> GetFilesWithTags(ICollection<TagModel> tags)
        {
            return _db.Files.Where(file => file.FileTags.All(filetag => tags.Contains(filetag.Tag))).ToList();
        }
    }
}

