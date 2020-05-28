using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class Test
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = -1;

        public string Using { get; set; }

        public string Classes { get; set; }

        [NotNull]
        public string CheckFunc { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Launch> Launches { get; set; }
    }
}
