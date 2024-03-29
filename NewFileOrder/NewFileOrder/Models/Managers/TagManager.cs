﻿using Microsoft.EntityFrameworkCore;
using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewFileOrder.Models.Managers
{
    public class TagManager : Manager
    {
        public TagManager(MyDbContext dbContext) : base(dbContext) { }

        public List<TagModel> GetTagsByName(ICollection<string> names, bool includeFileTags = false)
        {
            List<TagModel> tags = new List<TagModel>();
            bool tagError = false;
            StringBuilder errMsg = new StringBuilder("Nastaly následující chyby se zadanými tagy:\n");

            IQueryable<TagModel> dbTable;

            if (includeFileTags)
                dbTable = _db.Tags.Include(t => t.FileTags);
            else
                dbTable = _db.Tags;

            foreach (string name in names)
            {
                var tm = dbTable.Where(t => t.Name.Equals(name));
                if (tm.Count() == 1)
                    tags.Add(tm.First());
                else
                {
                    tagError = true;
                    if (tm.Count() == 0)
                        errMsg.AppendLine($"- zadaný tag {name} neexistuje");
                    else
                        errMsg.AppendLine($"- zadaný tag {name} má duplicitu, v databázi se nachází {tm.Count()}x, vyřeště prosím tento problém manuální úpravou databáze");
                }
            }
            if (tagError)
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

        public List<TagModel> GetAllTags(bool includeFileTags = false)
        {
            IQueryable<TagModel> dbTable;

            if (includeFileTags)
                dbTable = _db.Tags.Include(t => t.FileTags);
            else
                dbTable = _db.Tags;

            return Enumerable.OrderBy(dbTable, x => x.Name).ToList();
        }

        public void AddTag(string name)
        {
            _db.Tags.Add(new TagModel { Name = name });
            _db.SaveChanges();
        }

        public void RemoveTag(TagModel tag)
        {
            _db.Tags.Remove(tag);
            _db.SaveChanges();
        }

        public void UpdateTagsInDb(List<TagModel> tags)
        {
            _db.Tags.UpdateRange(tags);
            _db.SaveChanges();
        }
    }
}
