using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class ExpectedOutput
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = -1;

        [Unique, NotNull]
        public string Output { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Launch> Launches { get; set; }
    }
}
