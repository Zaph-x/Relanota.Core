using Core.Interfaces;
using Core.SqlHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Objects.Entities
{
    public class SuperTag : ISqlEntity
    {
        [Key]
        public int Key { get; set; }
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public string Name { get; set; }

        public void Delete(Database context, Action<string, string> callback)
        {
            throw new NotImplementedException();
        }

        public bool IsInContext(Database context)
        {
            throw new NotImplementedException();
        }

        public void Save(string content, string name, Database context)
        {
            throw new NotImplementedException();
        }

        public void Save(Database context)
        {
            throw new NotImplementedException();
        }

        public void Update(string content, string name, Database context)
        {
            throw new NotImplementedException();
        }
    }
}
