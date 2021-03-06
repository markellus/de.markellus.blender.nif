﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace De.Markellus.Blender.Nif.Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            IniFile iniConfig = new IniFile("../config.ini");
            string strName = iniConfig.GetVar("addon_name");
            string strSrc = "../" + iniConfig.GetVar("source_folder");
            string strDeps = "../" + iniConfig.GetVar("pyffi_folder");
            string strBlender = iniConfig.GetVar("blender_folder");
            string version = "unknown";

            try
            {
                string strInit = strSrc + "/" + strName + "/" + "/__init__.py";
                version = File.ReadAllText(strSrc + "/" + strName + "/" + "VERSION").Replace("\r\n", "");
                string[] strVersion = version.Split('.');

                string[] initFile = File.ReadAllLines(strInit);
                for (int i = 0; i < initFile.Length; i++)
                {
                    if (initFile[i].Contains("\"version\":"))
                    {
                        initFile[i] = "    \"version\": (" + strVersion[0] + ", " + strVersion[1] + ", " + strVersion[2] + "),";
                    }
                }
                File.Delete(strInit);
                File.WriteAllLines(strInit, initFile);
            }
            catch
            {
                Console.WriteLine("Failed to retrieve version!");
            }

            string strTarget = strName + "-" + version + ".zip";

            using (var memoryStream = new MemoryStream())
            {
                ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
                //var root = archive.CreateEntry(strName + "/");
                DirectoryCopy(strSrc, "", true, archive);
                DirectoryCopy(strDeps, strName + "/dependencies/pyffi", true, archive);

                archive.Dispose();
                if (File.Exists(strTarget))
                {
                    File.Delete(strTarget);
                }

                File.WriteAllBytes(strTarget, memoryStream.ToArray());
            }

            if (strBlender != "")
            {
                string strAddon = strBlender + "/" + strName;
                if (Directory.Exists(strAddon))
                {
                    Directory.Delete(strAddon, true);
                }
                ZipFile.ExtractToDirectory(strTarget, strBlender);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, ZipArchive archive)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                var entry = archive.CreateEntry(temppath, CompressionLevel.Optimal);
                using (var entryStream = entry.Open())
                using (var streamWriter = new BinaryWriter(entryStream))
                {
                    byte[] data = File.ReadAllBytes(file.FullName);
                    streamWriter.Write(data, 0, data.Length);
                }
            }

            //If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, archive);
                }
            }
        }
    }
}
