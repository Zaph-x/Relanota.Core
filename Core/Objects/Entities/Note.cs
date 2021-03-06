using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.SqlHelper;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Core.ExtensionClasses;

namespace Core.Objects.Entities
{
    public class Note : IComparable, ISqlEntity
    {
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public bool HasChanges { get; set; }
        public List<NoteTag> NoteTags { get; set; } = new List<NoteTag>();

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return Key.CompareTo((obj as Note).Key);
        }

        public void AddTag(Tag tag, Database context)
        {
            context.TryGetNoteTag(this, tag, out NoteTag noteTag);
            this.NoteTags.Add(noteTag);
            context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
            context.Update(this);
            context.SaveChanges();
        }

        public void RemoveTag(Tag tag, Database context)
        {
            context.TryGetNoteTag(this, tag, out NoteTag noteTag);

            this.NoteTags.RemoveAll(nt => nt.TagKey == tag.Key);

            context.TryUpdateManyToMany(context.NoteTags.Where(nt => nt.NoteKey == Key), this.NoteTags, x => x.TagKey);
            context.SaveChanges();
        }


        public void CheckInlineTags(Match match, Database context)
        {
            context.TryGetTag(match.Groups[1].Value, out Tag tag);
            this.AddTag(tag, context);
            context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
            context.SaveChanges();
        }

        public void Update(string content, string name, Database context)
        {
            if (context.Notes.FirstOrDefault(n => n.Key == this.Key) is Note entry)
            {
                this.Name = name.Trim();
                this.Content = content.Trim();
                this.NoteTags = context.NoteTags.Where(nt => nt.NoteKey == this.Key).Include(nt => nt.Tag).ToList();
                context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
                context.Entry(entry).CurrentValues.SetValues(this);
                context.SaveChanges();
            }
        }

        public void Update(Database context)
        {

            if (context.Notes.FirstOrDefault(n => n.Key == this.Key) is Note entry)
            {
                this.NoteTags = context.NoteTags.Where(nt => nt.NoteKey == this.Key).Include(nt => nt.Tag).ToList();
                context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
                context.Entry(entry).CurrentValues.SetValues(this);
                context.SaveChanges();
            }

        }

        public void Save(string content, string name, Database context)
        {
            this.Name = name.Trim();
            this.Content = content.Trim();
            context.Notes.Add(this);
            this.NoteTags = context.NoteTags.Where(nt => nt.NoteKey == this.Key).Include(nt => nt.Tag).ToList();
            context.TryUpdateManyToMany(this.NoteTags, this.NoteTags, x => x.TagKey);
            context.SaveChanges();
        }

        public static bool TryDeserialize(string serializedString, out Note note)
        {
            note = null;
            Match match = Regex.Match(serializedString, @"\|(.+?)\|(.+?)\|");
            if (match.Success)
            {
                note = new Note();
                try {
                    note.Name = match.Groups[1].Value.Decompress();
                    note.Content = match.Groups[2].Value.Decompress();
                }
                catch {
                    try {
                        if (Convert.FromBase64String(match.Groups[1].Value) is byte[] noteName &&
                            Convert.FromBase64String(match.Groups[2].Value) is byte[] noteContent) {
                            note.Name = Encoding.Default.GetString(noteName);
                            note.Content = Encoding.Default.GetString(noteContent);
                        }
                    }
                    catch {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryGetFullNote(Database context, out Note note)
        {
            note = context.Notes.Include(n => n.NoteTags).ThenInclude(nt => nt.Tag).FirstOrDefault(n => n.Key == this.Key);
            return note != null;
        }

        public bool IsInContext(Database context)
        {
            return context.Notes.AsEnumerable().Any(n => n.Key == this.Key || n.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Delete(Database context, Action<string, string> callback)
        {
            try
            {
                context.Notes.Remove(this);
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

        public string GetImportLink() => $"note://import/|{Name.Compress()}|{Content.Compress()}|";

        public void Save(Database context)
        {
            Save(this.Content, this.Name, context);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}