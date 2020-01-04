using Microsoft.EntityFrameworkCore;
using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewFileOrder.Models.Managers
{
    class TagManager:Manager
    {
        public TagManager(MyDbContext dbContext) : base(dbContext) { }

        public List<TagModel> GetTagsByName(ICollection<string> names)
        {
            List<TagModel> tags = new List<TagModel>();
            bool allTagsExist = true;
            StringBuilder errMsg = new StringBuilder("Nastaly následující chyby se zadanými tagy:\n");

            foreach (string name in names)
            {
                var tm = _db.Tags.Include(t => t.FileTags).Where(t => t.Name.Equals(name));
                if(tm.Count() == 1)
                    tags.Add(tm.First());
                else
                {
                    allTagsExist = false;
                    if (tm.Count() == 0)
                        errMsg.AppendLine($"- zadaný tag {name} neexistuje");
                    else
                        errMsg.AppendLine($"- zadaný tag {name} má duplicitu, v databázi se nachází {tm.Count()}x, vyřeště prosím tento problém manuální úpravou databáze");
                }
            }
            if (!allTagsExist)
                throw new Exception(errMsg.ToString());
            return tags;
        }

        public List<TagModel> GetTagsFromFileTags(ICollection<FileTag> fileTags)
        {
            List<TagModel> tags = new List<TagModel>();

            foreach (FileTag ft in fileTags)
                tags.Add(ft.Tag);
            return tags;
        }
    }
}
