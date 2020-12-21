using Core.Objects.Wrappers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Test.Objects.Wrappers
{
    public class FixedSizeObservableCollection_Test
    {
        [Test]
        public void Test_FixedSizeObservableCollection_CanOnlyStoreFixedSize()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);

            for (int i = 0; i < 20; i++)
                collection.Add(i.ToString());

            Assert.AreEqual(5, collection.Count, "Collection does not respect the fixed size constraint");
        }

        [Test]
        public void Test_FixedSizeObservableCollection_LastReturnsLastItemInList()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);
            for (int i = 0; i < 20; i++)
            {
                collection.Add(i.ToString());
                Assert.AreEqual(i.ToString(), collection.Last, "Last did not return the last instance in list");
            }

        }


        [Test]
        public void Test_FixedSizeObservableCollection_FirstReturnsFirstItemInList()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);
            for (int i = 0; i < 20; i++)
            {
                collection.Add(i.ToString());
                Assert.AreEqual((i < 5 ? "0" : (i-4).ToString()), collection.First, "First did not return the first instance in list");
            }
        }

        [Test]
        public void Test_FixedSizeObservableCollection_AddingAppendsToList()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);

            for (int i = 0; i < 20; i++)
                collection.Add(i.ToString());

            Assert.AreEqual("19", collection.Last);
        }

        [Test]
        public void Test_FixedSizeObservableCollection_OnlyOneExistsIfUnique()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);

            for (int i = 0; i < 20; i++)
            {
                collection.Add("test", true);
            }

            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_FixedSizeObservableCollection_InsertCanInsert()
        {
            FixedSizeObservableCollection<string> collection = new FixedSizeObservableCollection<string>(5);
            for (int i = 0; i < 20; i++)
            {
                collection.Insert(i.ToString());
            }

            Assert.AreEqual("19", collection[0], "Item was added, not inserted.");


        }
    }
}
