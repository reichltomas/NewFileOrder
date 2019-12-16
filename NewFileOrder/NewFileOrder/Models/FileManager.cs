using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NewFileOrder.Models
{
    class FileManager
    {
        private SHA256 _hasher = SHA256.Create();
        private MyDbContext db;
        void PutFileInDB(FileModel file)
        {
            db.Files.Add(file);
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
                db.Files.AddRange(list);
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
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
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

    }
}

