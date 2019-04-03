using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace De.Markellus.Blender.Nif.Builder
{
    //Liest eine Ini-Datei aus und ermöglicht dessen Verarbeitung
    public class IniFile
    {
        private string iniFile;
        private bool isInitialized = false;
        private string buffer;

        private string lineSplitter = "\r\n";
        private string valueSplitter = "=";

        private Dictionary<string, string> entrys;

        public bool Initialized
        {
            get { return isInitialized; }
        }

        public int EntryCount
        {
            get { return entrys.Count; }
        }

        public Dictionary<string, string> Entrys
        {
            get { return entrys; }
            set { entrys = value; }
        }

        public IniFile(string iniFile)
        {
            this.iniFile = iniFile;
            Initialize();
        }

        public string GetVar(string name)
        {
            try
            {
                return GetSubVars(entrys[name]);
            }
            catch
            {
                return "";
            }
        }

        public string GetSubVars(string var)
        {
            bool subVarFound = true;
            while (subVarFound)
            {
                int start = var.IndexOf('[') + 1;
                int end = var.IndexOf(']');
                if (start == -1 | end == -1 | start == end)
                {
                    subVarFound = false;
                }
                else
                {
                    string subVar = var.Substring(start, end - start);
                    if (GetVar(subVar) != "")
                    {
                        var = var.Replace("[" + subVar + "]", GetVar(subVar));
                    }
                }

            }

            return var;
        }

        public void ModifyEntry(string key, string value)
        {
            if(entrys.ContainsKey(key))
            {
                entrys[key] = value;
            }
            else
            {
                entrys.Add(key, value);
            }

            Save();
        }

        public void Save()
        {
            StringBuilder save = new StringBuilder();

            foreach(KeyValuePair<string,string> pair in entrys)
            {
                save.AppendLine(pair.Key + "=" + pair.Value);
            }

            File.WriteAllText(iniFile,save.ToString());
        }

        public void Reload()
        {
            isInitialized = false;
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) { return; }

            entrys = new Dictionary<string, string>();

            if (File.Exists(iniFile))
            {
                buffer = File.ReadAllText(iniFile);
                ProcessFile();
            }
            else
            {
                throw new Exception("file not found: " + iniFile);
            }
        }

        private void ProcessFile()
        {

            foreach (string line in buffer.Split(lineSplitter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("//") || line == "") continue;

                string[] entry = line.Split(valueSplitter.ToCharArray(), 2);

                entrys.Add(entry[0], entry[1]);
            }

            isInitialized = true;
        }
    }
}
