using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClockDisp
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class DescAttribute : Attribute
    {
        public DescAttribute(string desc)
        {
            Description = desc;
        }

        public string Description { get; private set; }
    }

    internal static class ConfigData
    {
        private const string PATH = "config.ini";
        private static Type type;
        private static FieldInfo[] fields;

        #region FIELDS

        [Desc("Last selected COM port name")]
        public static string LastSelectedCOMPort = "COM1";

        [Desc("Clock display render delay in ms")]
        public static int ClockDisplayDelay = 100;

        #endregion

        static ConfigData()
        {
            type = typeof(ConfigData);
            fields = type.GetFields();
        }

        public static void Load()
        {
            if (File.Exists(PATH))
            {
                // load
                string[] txt = File.ReadAllLines(PATH);
                for (int i = 0; i < txt.Length; i++)
                {
                    if (txt[i].Length == 0 || txt[i][0] == ';')
                        continue;

                    if (!txt[i].Contains("="))
                        continue;

                    string[] txtspl = txt[i].Split('=');
                    string name = txtspl[0].Trim();
                    string value = txtspl[1].Trim();

                    FieldInfo f = fields.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (f == null)
                    {
#if DEBUG
                        Trace.TraceWarning("Field name '" + name + " not found.");
#endif
                        continue;
                    }
                    if (f.FieldType.Name == nameof(Int32))
                    {
                        f.SetValue(type, int.Parse(value));
                    }
                    else
                    {
                        f.SetValue(type, value);
                    }
                }
                Apply();
            }
            else
            {
                // save default
                Save();
            }
        }

        public static void Save()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < fields.Length; i++)
            {
                DescAttribute da = fields[i].GetCustomAttribute<DescAttribute>(false);
                string name = fields[i].Name;
                object value = fields[i].GetValue(type);
                string valueType = fields[i].FieldType.Name;

                sb.Append($"\n; {da.Description} [{valueType}]\n{name} = ");
                sb.Append(value);
                sb.AppendLine();
            }

            File.WriteAllText(PATH, sb.ToString());
        }

        private static void Apply()
        {
            if (ClockDisplayDelay < 2)
                ClockDisplayDelay = 2;
            else if (ClockDisplayDelay > 500)
                ClockDisplayDelay = 500;
        }

    }
}
