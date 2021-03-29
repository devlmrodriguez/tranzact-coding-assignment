using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using TranzactCodingAssignment.Models;

namespace TranzactCodingAssignment.Services
{
    public class PageViewFileService
    {
        public List<PageView> PageViews { get; private set; }

        public PageViewFileService()
        {
            PageViews = new List<PageView>();
        }

        public void DownloadFile(string uriName, string fileName)
        {
            Console.WriteLine($"Downloading {fileName}");

            WebClient webClient = new WebClient();
            webClient.DownloadFile(uriName, fileName);

            Console.WriteLine($"Downloaded {fileName}");
            Console.WriteLine();
        }

        public void DecompressFile(string fileName, string decompressedFileName)
        {
            Console.WriteLine($"Decompressing {fileName}");

            //Decompress our file with GZipStream
            FileInfo fileInfo = new FileInfo(fileName);
            using (FileStream originalFileStream = fileInfo.OpenRead())
            {
                using (FileStream decompressedFileStream = File.Create(decompressedFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        decompressionStream.CopyTo(decompressedFileStream);
                }
            }

            Console.WriteLine($"Decompressed {fileName}");
            Console.WriteLine();
        }

        public void ReadFile(string decompressedFileName)
        {
            Console.WriteLine($"Reading {decompressedFileName}");

            PageViews.Clear();
            string line = null;

            //Read page views line by line from decompressed file
            using (StreamReader fileStreamReader = new StreamReader(decompressedFileName))
            {
                while ((line = fileStreamReader.ReadLine()) != null)
                {
                    try
                    {
                        //Split line with space separator and try getting 4 tokens
                        string[] tokens = line.Split(" ");

                        //Parse tokens to relevant data
                        string domainCode = tokens[0];
                        string[] domainCodeTokens = domainCode.Split(".");
                        string language = domainCodeTokens.First();
                        //Only care about last domain
                        string domain = domainCodeTokens.Last();
                        //If they are equal, there is no domain, therefore, put a default one
                        if (language == domain)
                            domain = "";
                        string pageTitle = tokens[1];
                        int countViews = int.Parse(tokens[2]);
                        int totalResponseSize = int.Parse(tokens[3]);

                        PageView newPageView = new PageView(language, domain, pageTitle, countViews);
                        PageViews.Add(newPageView);
                    }
                    catch (Exception)
                    {
                        //Bad format could've happened in earlier wikipedia page views
                    }
                }
            }

            Console.WriteLine($"Read {decompressedFileName}");
            Console.WriteLine();
        }

        public void CleanFiles(string fileName, string decompressedFileName)
        {
            Console.WriteLine($"Cleaning {fileName}");

            //Delete previous used files if they still exist
            if (File.Exists(fileName))
                File.Delete(fileName);

            if (File.Exists(decompressedFileName))
                File.Delete(decompressedFileName);

            PageViews.Clear();

            Console.WriteLine($"Cleaned {fileName}");
            Console.WriteLine();
        }
    }
}
