using Core.ExtensionClasses;
using Core.Objects.Entities;
using Core.SqlHelper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Core.Tests.SqlHelper
{
    public class Database_Test
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Constants.DATABASE_PATH = $"{TestContext.CurrentContext.TestDirectory}/assets/";
            Constants.DATABASE_NAME = "Test_Database_DB.db";
            if (!Directory.Exists(Constants.DATABASE_PATH)) Directory.CreateDirectory(Constants.DATABASE_PATH);

            using Database database = new Database();
            
            database.Database.EnsureCreated();
            database.Database.Migrate();

            database.Notes.Clear();
            database.Tags.Clear();
            database.NoteTags.Clear();

            database.SaveChanges();
        }

        [Test]
        public void Test_Database_CanCreateDatabase()
        {
            using (Database context = new Database(Constants.DATABASE_PATH, $"Test_Database_CanCreateDatabase"))
            {
                context.Database.EnsureCreated();

                Assert.IsTrue(File.Exists($"{Constants.DATABASE_PATH}{"Test_Database_CanCreateDatabase"}"), "Database was not created");
                
                context.Database.EnsureDeleted();

                Assert.IsFalse(File.Exists($"{Constants.DATABASE_PATH}{"Test_Database_CanCreateDatabase"}"), "Database was not deleted again");
            }
        }

        [Test]
        public void Test_Database_CanNotInsertTwoNotesWithSameName()
        {
            using (Database context = new Database())
            {

                Note note = new Note();
                note.Save("TestContent", "Test_Database_CanNotInsertTwoNotesWithSameName", context);

                Note _note = new Note();
                Assert.Throws<DbUpdateException>(() => note.Save("TestContentFromNewNote", "Test_Database_CanNotInsertTwoNotesWithSameName", context), "Constraint on name not respected");

            }
        }

        [Test]
        public void Test_Database_NoteInContextReturnsTrueOnNoteInContext()
        {
            using (Database context = new Database())
            {
                Note note = new Note();
                note.Save("TestContent", "Test_Database_NoteInContextReturnsTrueOnNoteInContext", context);

                Assert.IsTrue(note.IsInContext(context), "Note was not added to the context");
            }

        }

        [Test]
        public void Test_Database_CanInsertNoteWithTag()
        {
            using (Database context = new Database())
            {
                Note note = new Note();
                note.Save("TestContent", "Test_Database_CanInsertNoteWithTag_NOTE", context);
                Tag tag = new Tag { Name = " Test_Database_CanInsertNoteWithTag_TAG", Description = "TestDesc" };
                note.AddTag(tag, context);


                Note assertableNote = context.Notes.AsEnumerable().FirstOrDefault(note => note.Name == "Test_Database_CanInsertNoteWithTag_NOTE");
                assertableNote.TryGetFullNote(context, out Note fullNote);

                Assert.IsNotEmpty(fullNote.NoteTags, "Note did not have 1 tag");
                Assert.AreEqual(1, fullNote.NoteTags.Count, "Note did not have 1 tag");
            }
        }


        [Test]
        public void Test_Database_TryGetNoteReturnsNote()
        {
            using (Database context = new Database())
            {
                Note note = new Note { Name = $"Test_Database_TryGetNoteReturnsNote", Content = "testNote" };
                note.Save(context);

                context.TryGetNote($"Test_Database_TryGetNoteReturnsNote", out Note extractedNote);
                Assert.IsNotNull(extractedNote, $"Could not full note");
            }
        }

        [Test]
        public void Test_Database_TryGetNoteReturnsNoteFromKey()
        {
            using (Database context = new Database())
            {
                Note note = new Note { Name = $"Test_Database_TryGetNoteReturnsNoteFromKey", Content = "testNote" };
                note.Save(context);

                context.TryGetNote(1, out Note extractedNote); // Make sure to get key 1. Anything else is not certain
                Assert.IsNotNull(extractedNote, $"Could not full note");
            }
        }

        [Test]
        public void Test_Database_TryGetNoteReturnsFullNote()
        {
            using (Database context = new Database())
            {
                Note note = new Note { Name = $"Test_Database_TryGetNoteReturnsFullNote", Content = "testNote" };
                Tag tag = new Tag { Name = $"Test_Database_TryGetNoteReturnsFullNote_tag", Description = "" };
                note.Save(context);
                note.AddTag(tag, context);

                context.TryGetNote($"Test_Database_TryGetNoteReturnsFullNote", out Note extractedNote);
                Assert.AreEqual(1, extractedNote.NoteTags.Count, $"Could not get full note");
            }
        }

        [Test]
        public void Test_Database_TryGetTagGetsTagFromDatabase()
        {
            using (Database context = new Database())
            {
                Tag tag = new Tag { Name = $"Test_Database_TryGetTagGetsTagFromDatabase", Description = "testTag" };
                tag.Save(context);

                context.TryGetTag("Test_Database_TryGetTagGetsTagFromDatabase", out Tag extractedTag);
                Assert.IsNotNull(extractedTag, "Tag could not be extracted from data context");
            }

        }

    }
}