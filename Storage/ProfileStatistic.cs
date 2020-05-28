using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class ProfileStatistic
    {
        [NotNull, ForeignKey(typeof(Test))]
        public int LaunchId { get; set; }
        [ManyToOne]
        public Launch Launch { get; set; }

        public double Time { get; set; }
        public float CpuUsage { get; set; }
        public float RamUsage { get; set; }
    }
}
