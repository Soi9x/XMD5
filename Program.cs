using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace XMD5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(currentProcessName).Length > 1)
            {
                Console.WriteLine("XMD5 is already running. Exiting...");
                Environment.Exit(0); // Prevent double process
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: XMD5 -create|-verify <directoryPath>");
                return;
            }

            string command = args[0];
            string directoryPath = args[1];

            if (command == "-create")
            {
                CreateMD5Hashes(directoryPath);
            }
            else if (command == "-verify")
            {
                VerifyMD5Hashes(directoryPath);
            }
            else
            {
                Console.WriteLine("Invalid command. Use -create or -verify.");
            }
        }

        static void CreateMD5Hashes(string directoryPath)
        {
            string jsonOutputPath = Path.Combine(directoryPath, "VRFMD5.json");

            var fileHashes = new Dictionary<string, string>();
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;

            for (int i = 0; i < totalFiles; i++)
            {
                string filePath = files[i];
                string relativePath = filePath.Substring(directoryPath.Length + 1);

                // Bỏ qua tệp XMD5.exe và VRFMD5.json
                if (relativePath.Equals("XMD5.exe", StringComparison.OrdinalIgnoreCase) ||
                    relativePath.Equals("VRFMD5.json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string hash = GetMD5Hash(filePath);
                fileHashes[relativePath] = hash;

                // Báo cáo phần trăm tiến trình
                ReportProgress(i + 1, totalFiles);
            }

            string json = JsonConvert.SerializeObject(fileHashes, Formatting.Indented);
            File.WriteAllText(jsonOutputPath, json);

            Console.WriteLine("MD5 hashes have been generated and saved to JSON.");
        }

        static void VerifyMD5Hashes(string directoryPath)
        {
            string jsonInputPath = Path.Combine(directoryPath, "VRFMD5.json");

            if (!File.Exists(jsonInputPath))
            {
                Console.WriteLine("JSON file not found.");
                return;
            }

            string json = File.ReadAllText(jsonInputPath);
            var fileHashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            bool allMatch = true;
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;

            for (int i = 0; i < totalFiles; i++)
            {
                string filePath = files[i];
                string relativePath = filePath.Substring(directoryPath.Length + 1);

                // Bỏ qua tệp XMD5.exe và VRFMD5.json
                if (relativePath.Equals("XMD5.exe", StringComparison.OrdinalIgnoreCase) ||
                    relativePath.Equals("VRFMD5.json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string currentHash = GetMD5Hash(filePath);

                if (fileHashes.ContainsKey(relativePath))
                {
                    string expectedHash = fileHashes[relativePath];
                    if (currentHash == expectedHash)
                    {
                        Console.WriteLine($"{relativePath}: Hash matches.");
                    }
                    else
                    {
                        Console.WriteLine($"{relativePath}: Hash does not match!");
                        allMatch = false;
                    }
                }
                else
                {
                    Console.WriteLine($"{relativePath}: No hash found in JSON.");
                    allMatch = false;
                }

                // Báo cáo phần trăm tiến trình
                ReportProgress(i + 1, totalFiles);
            }

            if (allMatch)
            {
                Console.WriteLine("Match");
            }
            else
            {
                Console.WriteLine("Not Match");
            }
        }

        static void ReportProgress(int current, int total)
        {
            double percentage = (double)current / total * 100;
            Console.WriteLine($"Calculate: {percentage:F2}%");
        }

        static string GetMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}