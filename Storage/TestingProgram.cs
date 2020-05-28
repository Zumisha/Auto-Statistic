using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class TestingProgram
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = -1;

        [NotNull]
        public byte[] ProgramSha512Hash { get; set; }

        public string EnvironmentVersion { get; set; }

        [NotNull, ForeignKey(typeof(User))]
        public int UserId { get; set; }
        [ManyToOne]
        public User User { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Launch> Launches { get; set; }
    }
}
