using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Interfaces;
using Core.SqlHelper;
using Microsoft.EntityFrameworkCore;

namespace Core.Objects.Entities
{
    public class Tag : ISqlEntity
    {
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<NoteTag> NoteTags { get; set; } = new List<NoteTag>();

        public void Save(string description, string name, Database context)
        {
            this.Description = description.Trim();
            this.Name = name.Trim();
            context.Tags.Add(this);
            this.NoteTags = context.NoteTags.Where(nt => nt.NoteKey == this.Key).Include(nt => nt.Tag).ToList();
            context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
            context.SaveChanges();
        }

        public void Update(string description, string name, Database context)
        {
            if (context.Tags.FirstOrDefault(t => t.Key == this.Key) is Tag tag)
            {
                this.Name = name.Trim();
                this.Description = description.Trim();
                context.Entry(tag).CurrentValues.SetValues(this);
                context.SaveChanges();
            }


        }

        public void Delete(Database context, Action<string, string> callback)
        {
            try
            {
                context.Tags.Remove(this);
                context.SaveChanges();
            }
            catch (Exception e)
            {
#if DEBUG
                callback("An exception occoured", e.Message);
#endif
                return;
            }
        }

        public bool IsInContext(Database context)
        {
            return context.Tags.AsEnumerable().Any(t => t.Key == this.Key || t.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Save(Database context)
        {
            this.Save(this.Description, this.Name, context);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}