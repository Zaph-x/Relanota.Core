using System;
using System.ComponentModel.DataAnnotations;
using Core.SqlHelper;

namespace Core.Interfaces
{
    public interface ISqlEntity
    {
        [Key]
        int Key { get; set; }
        void Save(string content, string name, Database context);
        void Save(Database context);
        void Update(string content, string name, Database context);
        void Delete(Database context, Action<string, string> callback);
        bool IsInContext(Database context);
    }
}