using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = -1;

        [Unique, NotNull]
        public string Mail { get; set; }

        [NotNull]
        public byte[] Hash { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TestingProgram> TestingPrograms { get; set; }
    }
}
