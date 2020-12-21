using Core.ExtensionClasses;
using Core.Objects.Entities;
using Core.SqlHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Test.Objects.Entities
{
    class Note_Test
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Constants.DATABASE_PATH = $"{TestContext.CurrentContext.TestDirectory}/assets/";
            Constants.DATABASE_NAME = "Test_DB.db";
            using Database context = new Database();

            context.Database.EnsureCreated();
            context.Notes.Clear();
            context.Tags.Clear();
            context.NoteTags.Clear();
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
