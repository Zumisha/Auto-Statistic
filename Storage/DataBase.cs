using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Auto_Statistic.Storage
{
    public static class DataBase
    {
        //static readonly string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Statistic.Db");
        public static readonly SQLiteConnection Db = new SQLiteConnection("Statistic.Db");

        public static void Create()
        {
            Db.CreateTable<User>();
            Db.CreateTable<TestingProgram>();
            Db.CreateTable<Test>();
            Db.CreateTable<LaunchParams>();
            Db.CreateTable<ExpectedOutput>();
            Db.CreateTable<Launch>();
            Db.CreateTable<ProfileStatistic>();
        }
    }
}
