using Core.ExtensionClasses;
using Core.Objects.Entities;
using Core.SqlHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Core.Test.Objects.Entities
{
    class Note_Test
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Constants.DATABASE_PATH = $"{TestContext.CurrentContext.TestDirectory}/assets/";
            Constants.DATABASE_NAME = "Test_DB.db";
            Directory.Delete(Constants.DATABASE_PATH, true);
            if (!Directory.Exists(Constants.DATABASE_PATH)) Directory.CreateDirectory(Constants.DATABASE_PATH);
            using Database database = new Database();
            database.Database.EnsureCreated();

            database.Notes.Clear();
            database.Tags.Clear();
            database.NoteTags.Clear();

            database.SaveChanges();

        }

        [Test]
        public void Test_Note_CanDeserialiseNoteFromDatabase()
        {
            Assert.IsTrue(Note.TryDeserialize("|dGVzdA==|dGVzdCBjb250ZW50|", out Note extractedNote), "Note was not extracted");

            Assert.IsNotNull(extractedNote, "Note was null");
            Assert.AreEqual("test", extractedNote.Name, "Name did not match expected name");
            Assert.AreEqual("test content", extractedNote.Content, "Content did not match expected");

        }

        [Test]
        public void Test_Note_FailsToDeserialiseNoteWhenIncorrectFormatIsPassed()
        {
            Assert.IsFalse(Note.TryDeserialize("This should fail", out Note extractedNote), "Note was unexpectedly extracted from non-base64 string");
            Assert.IsNull(extractedNote, "Note was not null");
        }

    }
}
