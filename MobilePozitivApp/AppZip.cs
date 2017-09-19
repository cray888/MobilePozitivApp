
using System;
using System.IO;
using System.IO.Compression;

namespace MobilePozitivApp
{
    public class AppZip
    {
        public static bool Unzip(string zipFile, string targetDirectory)
        {
            using (ZipArchive archive = new ZipArchive(File.Open(zipFile, FileMode.Open)))
            {
                double zipEntriesExtracted = 0;
                double zipEntries;

                zipEntries = archive.Entries.Count;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    try
                    {
                        string fullPath = Path.Combine(targetDirectory, entry.FullName);
                        if (String.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        else
                        {
                            var destFileName = Path.Combine(targetDirectory, entry.FullName);

                            using (var fileStream = File.Create(destFileName))
                            {
                                using (var entryStream = entry.Open())
                                {
                                    entryStream.CopyTo(fileStream);
                                }
                            }
                        }
                        zipEntriesExtracted++;
                    }
                    catch// (Exception ex)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}