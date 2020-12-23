using System;
using System.Collections.Generic;
using System.Linq;
using Core.Objects.Entities;
using Microsoft.EntityFrameworkCore;


namespace Core.SqlHelper
{
    public class Database : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }
        public DbSet<SuperTag> SuperTags { get; set; }
        public string path = "./";
        public string name = "notes.db";
        public string ConnectionString => $"Data Source={path}/{name}";

        public Database(DbContextOptions options) :base(options)
        {

            path = Constants.DATABASE_PATH;
            name = Constants.DATABASE_NAME;
        }

        public Database() : base()
        {
            this.path = Constants.DATABASE_PATH;
            this.name = Constants.DATABASE_NAME;
        }
        public Database(string path, string name) : base()
        {
            this.path = path;
            this.name = name;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
            base.OnConfiguring(optionsBuilder);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Note>().HasKey(nt => nt.Key);
            modelBuilder.Entity<Tag>().HasKey(t => t.Key);
            modelBuilder.Entity<SuperTag>().HasKey(st => st.Key);
            modelBuilder.Entity<NoteTag>().HasKey(nt => new { nt.NoteKey, nt.TagKey });

            modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();
            modelBuilder.Entity<Note>().HasIndex(nt => nt.Name).IsUnique();
            modelBuilder.Entity<SuperTag>().HasIndex(st => st.Name).IsUnique();


            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Note)
                .WithMany(note => note.NoteTags)
                .HasForeignKey(nt => nt.NoteKey);

            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NoteTags)
                .HasForeignKey(nt => nt.TagKey);

            modelBuilder.Entity<SuperTag>()
                .HasMany(s => s.Tags)
                .WithOne();
        }


        public bool TryGetNote(string name, out Note note)
        {
            note = this.Notes.FirstOrDefault(n => n.Name == name);
            return note != null;
        }

        public bool TryGetNote(int id, out Note note)
        {
            note = this.Notes.FirstOrDefault(n => n.Key == id);
            return note != null;
        }

        public bool TryGetTag(string tagName, out Tag tag)
        {
            tag = Tags.FirstOrDefault(t => t.Name.ToLower() == tagName.ToLower()) ?? new Tag() { Name = tagName };
            return tag != null;
        }

        public bool TryGetNoteTag(Note note, Tag tag, out NoteTag noteTag)
        {
            noteTag = NoteTags.FirstOrDefault(nt => nt.Note.Name.ToLower() == note.Name.ToLower() && nt.Tag.Name.ToLower() == tag.Name.ToLower())
                      ?? new NoteTag() { Note = note, Tag = tag };
            return noteTag != null;
        }
    }

    public static class DbHelper
    {
        public static void TryUpdateManyToMany<T, TKey>(this DbContext db, IEnumerable<T> currentItems, IEnumerable<T> newItems, Func<T, TKey> getKey) where T : class
        {
            db.Set<T>().RemoveRange(currentItems.Except(newItems, getKey));
            db.Set<T>().AddRange(newItems.Except(currentItems, getKey));
        }

        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKeyFunc)
        {
            return items
                .GroupJoin(other, getKeyFunc, getKeyFunc, (item, tempItems) => new { item, tempItems })
                .SelectMany(t => t.tempItems.DefaultIfEmpty(), (t, temp) => new { t, temp })
                .Where(t => ReferenceEquals(null, t.temp) || t.temp.Equals(default(T)))
                .Select(t => t.t.item);
        }


    }
}