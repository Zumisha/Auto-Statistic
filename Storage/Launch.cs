using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Auto_Statistic.Storage
{
    public class Launch
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = -1;

        [NotNull]
        public DateTime Date { get; set; }

        public string Log { get; set; }

        public ExecutionStatus Status { get; set; }

        [NotNull, ForeignKey(typeof(Test))]
        public int TestId { get; set; }
        [ManyToOne]
        public Test Test { get; set; }

        [NotNull, ForeignKey(typeof(TestingProgram))]
        public int TestingProgramId { get; set; }
        [ManyToOne]
        public TestingProgram TestingProgram { get; set; }

        [NotNull, ForeignKey(typeof(LaunchParams))]
        public int LaunchParamsId { get; set; }
        [ManyToOne]
        public LaunchParams LaunchParams { get; set; }

        [NotNull, ForeignKey(typeof(ExpectedOutput))]
        public int ExpectedOutputId { get; set; }
        [ManyToOne]
        public ExpectedOutput ExpectedOutput { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ProfileStatistic> ProfileStatistics { get; set; }
    }
}
