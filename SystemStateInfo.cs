using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Win32;

namespace FPTL_Auto_Statistic
{
    public static class SystemStateInfo
    {

        private static readonly PerformanceCounter availableRamSize = new PerformanceCounter("Memory", "Available MBytes");

        private static readonly PerformanceCounter totalCpuUsage = new PerformanceCounter("Process", "% Processor Time", "_Total");
        private static readonly PerformanceCounter idleCpuUsage = new PerformanceCounter("Process", "% Processor Time", "Idle");

        public static float AvailableRamSize()
        {
            return availableRamSize.NextValue();
        }

        public static float TotalCpuUsage()
        {
            return totalCpuUsage.NextValue();
        }

        public static float IdleCpuUsage()
        {
            return idleCpuUsage.NextValue();
        }
    }

    public static class FullSystemInfo
    {
        private static List<CpuInfo> CpuInfoList = null;
        private static void getCpuInfo()
        {
            List<string> properties = new List<string>()
            {
                "Name",
                "NumberOfCores",
                "NumberOfLogicalProcessors",
                "MaxClockSpeed",
                "AddressWidth",
                "DataWidth",
                "L2CacheSize",
                "L3CacheSize",
                "L4CacheSize"
            };
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            CpuInfoList = new List<CpuInfo>();
            foreach (var queryObj in searcher.Get())
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                foreach (var prop in properties)
                {
                    try
                    {
                        info.Add(prop,queryObj[prop]);
                    }
                    catch (Exception exc)
                    {
                        info.Add(prop, 0);
                    }
                }
                CpuInfoList.Add(new CpuInfo(
                    info["Name"].ToString(),
                    Convert.ToInt32(info["NumberOfCores"]),
                    Convert.ToInt32(info["NumberOfLogicalProcessors"]),
                    Convert.ToInt32(info["MaxClockSpeed"]),
                    Convert.ToInt32(info["AddressWidth"]),
                    Convert.ToInt32(info["DataWidth"]),
                    Convert.ToInt32(info["L2CacheSize"]),
                    Convert.ToInt32(info["L3CacheSize"]),
                    Convert.ToInt32(info["L4CacheSize"])
                ));
            }
        }
        public struct CpuInfo
        {
            public readonly string Name;
            public readonly int NumCores;
            public readonly int NumInstrThreads;
            public readonly int BaseFrequency;
            public readonly int L1InstrCacheSize;
            public readonly int L1DataCacheSize;
            public readonly int L2CacheSize;
            public readonly int L3CacheSize;
            public readonly int L4CacheSize;

            public CpuInfo(string cpuName, int numberOfCpuCores, int numberOfCpuInstructionThreads, int cpuBaseFrequency, int cpuL1InstructionCacheSize, int cpuL1DataCacheSize, int cpuL2CacheSize, int cpuL3CacheSize, int cpuL4CacheSize)
            {
                Name = cpuName;
                NumCores = numberOfCpuCores;
                NumInstrThreads = numberOfCpuInstructionThreads;
                BaseFrequency = cpuBaseFrequency;
                L1InstrCacheSize = cpuL1InstructionCacheSize;
                L1DataCacheSize = cpuL1DataCacheSize;
                L2CacheSize = cpuL2CacheSize;
                L3CacheSize = cpuL3CacheSize;
                L4CacheSize = cpuL4CacheSize;
            }

            public Dictionary<string, string> ToDictionary()
            {
                var info = new Dictionary<string, string>();

                foreach (var field in typeof(CpuInfo).GetFields())
                {
                    info.Add(field.Name, field.GetValue(this).ToString());
                }

                return info;
            }
        }
        public static List<CpuInfo> CPUsInfo
        {
            get
            {
                if (CpuInfoList == null) getCpuInfo();
                return CpuInfoList;
            }
        }

        //-------------------------------------------------------

        /// <summary>
        /// Returns the total amount of computer RAM in KBytes.
        /// </summary>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

        private static List<RamBoardInfo> RamBoardInfoList = null;
        private static RamInfo ramInfo = null;
        private static readonly Dictionary<int, string> RamTypes = new Dictionary<int, string>()
        {
            {0, "Unknown (0)"},
            {1, "Other (1)"},
            {2, "DRAM"},
            {3, "Synchronous DRAM"},
            {4, "Cache DRAM"},
            {5, "EDO"},
            {6, "EDRAM"},
            {7, "VRAM"},
            {8, "SRAM"},
            {9, "RAM"},
            {10, "ROM"},
            {11, "Flash"},
            {12, "EEPROM"},
            {13, "FEPROM"},
            {14, "EPROM"},
            {15, "CDRAM"},
            {16, "3DRAM"},
            {17, "SDRAM"},
            {18, "SGRAM"},
            {19, "RDRAM"},
            {20, "DDR"},
            {21, "DDR2"},
            {22, "DDR2 FB-DIMM"},
            {24, "DDR3"},
            {25, "FBD2"},
            {26, "DDR4"}
        };
        private static void getRamInfo()
        {
            RamBoardInfoList = new List<RamBoardInfo>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");

            int ramFrequency = 0;
            string ramType = "";

            foreach (var queryObj in searcher.Get())
            {
                var ramBoard = new RamBoardInfo(queryObj["PartNumber"].ToString(), Convert.ToInt64(queryObj["Capacity"]) / 1024 / 1024);
                ramFrequency = Convert.ToInt32(queryObj["Speed"]);
                ramType = RamTypeToString(Convert.ToInt32(queryObj["MemoryType"]));
                RamBoardInfoList.Add(ramBoard);
            }

            GetPhysicallyInstalledSystemMemory(out long totalRamSize);
            totalRamSize /= 1024;

            ramInfo = new RamInfo(totalRamSize, ramType, ramFrequency, 0);
        }
        private static string RamTypeToString(int type)
        {
            string outValue = null;
            if (RamTypes.ContainsKey(type))
                outValue = RamTypes[type];
            else
                outValue = "Undefined (" + type.ToString() + ")";
            return outValue;
        }
        public class RamInfo
        {
            public readonly long totalRamSizeMB;
            public readonly string ramType;
            public readonly int ramFrequency;
            public readonly int numberOfRamChannels;

            public RamInfo(long totalRamSizeMb, string ramType, int ramFrequency, int numberOfRamChannels)
            {
                this.totalRamSizeMB = totalRamSizeMb;
                this.ramType = ramType;
                this.ramFrequency = ramFrequency;
                this.numberOfRamChannels = numberOfRamChannels;
            }

            public Dictionary<string, string> ToDictionary()
            {
                var info = new Dictionary<string, string>();

                foreach (var field in typeof(RamInfo).GetFields())
                {
                    info.Add(field.Name, field.GetValue(this).ToString());
                }

                return info;
            }
        }
        public struct RamBoardInfo
        {
            public RamBoardInfo(string name, long size)
            {
                Name = name;
                Size = size;
            }
            public readonly string Name;
            public readonly long Size;

            public Dictionary<string, string> ToDictionary()
            {
                var info = new Dictionary<string, string>();

                foreach (var field in typeof(RamBoardInfo).GetFields())
                {
                    info.Add(field.Name, field.GetValue(this).ToString());
                }

                return info;
            }
        }
        public static List<RamBoardInfo> RamBoardsInfo
        {
            get
            {
                if (RamBoardInfoList == null) getRamInfo();
                return RamBoardInfoList;
            }
        }
        public static RamInfo GeneralRamInfo
        {
            get
            {
                if (ramInfo == null) getRamInfo();
                return ramInfo;
            }
        }

        //-------------------------------------------------------------------

        public static class OsInfo
        {
            public static string Name = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            public static int BitDepth = Environment.Is64BitOperatingSystem ? 64 : 32;
            public static string Version = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            public static string BuildVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentBuild", "").ToString();
            public static Dictionary<string, string> ToDictionary()
            {
                var info = new Dictionary<string, string>();

                foreach (var field in typeof(OsInfo).GetFields())
                {
                    info.Add(field.Name, field.GetValue(null).ToString());
                }

                return info;
            }
        }

        /*public static Dictionary<string, string> BiosInfo()
        {
            List<string> properties = new List<string>() { };
            Dictionary<string, string> result = new Dictionary<string, string>(properties.Count);
            string query = "SELECT * FROM Win32_BaseBoard";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject info in searcher.Get())
            {
                foreach (var property in properties)
                {
                    result[property] = info.GetPropertyValue(property).ToString();
                }
            }

            return result;
        }*/

        public static Dictionary<string, Dictionary<string, string>> Get()
        {
            var info = new Dictionary<string, Dictionary<string, string>>();
            
            info.Add("OS", OsInfo.ToDictionary());

            var i = 1;
            foreach (var cpuInfo in CPUsInfo)
            {
                info.Add("CPU " + i, cpuInfo.ToDictionary());
                ++i;
            }

            info.Add("RAM", GeneralRamInfo.ToDictionary());

            i = 1;
            foreach (var ramBoard in RamBoardsInfo)
            {
                info.Add("RAM board " + i, ramBoard.ToDictionary());
                ++i;
            }

            /*i = 1;
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_CacheMemory");
            foreach (var obj in searcher.Get())
            {
                Dictionary<string, string> result = new Dictionary<string, string>(obj.Properties.Count);
                foreach (var property in obj.Properties)
                {
                    if (property.Value != null)
                    result[property.Name] = property.Value.ToString();
                }
                info.Add("Win32_CacheMemory " + i, result);
                ++i;
            }*/

            return info;
        }
    }
}
