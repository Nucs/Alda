﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Common {
    /// <summary>
    ///     Class that determines paths.
    /// </summary>
    public static class Paths {
#pragma warning disable CS0169 // The field 'Paths._cacheprogress' is never used
        private static Task _cacheprogress;
#pragma warning restore CS0169 // The field 'Paths._cacheprogress' is never used

        private static readonly string _location = Process.GetCurrentProcess().MainModule.FileName;

        /// <summary>
        ///     Gives the path to windows dir, most likely to be 'C:/Windows/'
        /// </summary>
        public static DirectoryInfo WindowsDirectory => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System));

        /// <summary>
        ///     The path to the entry exe.
        /// </summary>
        public static FileInfo ExecutingExe => new FileInfo(_location);

        /// <summary>
        ///     The config dir inside user profile.
        /// </summary>
        public static DirectoryInfo ConfigDirectory => new DirectoryInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "autoload/"));

        /// <summary>
        ///     The config file inside user profile.
        /// </summary>
        public static FileInfo ConfigFile(string configname) => new FileInfo(Path.Combine(ConfigDirectory.FullName, Environment.MachineName +$".{configname}.json"));

        /// <summary>
        ///     The path to the entry exe's directory.
        /// </summary>
        public static DirectoryInfo ExecutingDirectory => ExecutingExe.Directory;

        public static DirectoryInfo CurrentDirectory => new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        ///     Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public static bool IsDirectoryWritable(this DirectoryInfo directory) {
            var success = false;
            var fullPath = directory + "testicales.exe";

            if (directory.Exists)
                try {
                    using (var fs = new FileStream(fullPath, FileMode.CreateNew,
                        FileAccess.Write)) {
                        fs.WriteByte(0xff);
                    }

                    if (File.Exists(fullPath)) {
                        File.Delete(fullPath);
                        success = true;
                    }
                } catch (Exception) {
                    success = false;
                }
            return success;
        }

        /// <summary>
        ///     Combines the file name with the dir of <see cref="Paths.ExecutingExe" />, resulting in path of a file inside the
        ///     directory of the executing exe file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static FileInfo CombineToExecutingBase(string filename) {
            if (ExecutingExe.DirectoryName != null)
                return new FileInfo(Path.Combine(ExecutingExe.DirectoryName, filename));
            return null;
        }

        /// <summary>
        ///     Combines the file name with the dir of <see cref="Paths.ExecutingExe" />, resulting in path of a file inside the
        ///     directory of the executing exe file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DirectoryInfo CombineToExecutingBaseDir(string filename) {
            if (ExecutingExe.DirectoryName != null)
                return new DirectoryInfo(Path.Combine(ExecutingExe.DirectoryName, filename));
            return null;
        }

        /// <summary>
        ///     Compares two FileSystemInfos the right way.
        /// </summary>
        /// <returns></returns>
        public static bool CompareTo(this FileSystemInfo fi, FileSystemInfo fi2) {
            return NormalizePath(fi.FullName).Equals(NormalizePath(fi2.FullName), StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Compares two FileSystemInfos the right way.
        /// </summary>
        /// <returns></returns>
        public static bool CompareTo(string fi, string fi2) {
            return NormalizePath(fi).Equals(NormalizePath(fi2), StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Normalizes path to prepare for comparison or storage
        /// </summary>
        public static string NormalizePath(string path) {
            path = path.Replace("/", "\\")
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
            if (path.Contains("\\"))
                if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                    try {
                        path = Path.GetFullPath(new Uri(path).LocalPath);
                    } catch { }
            //is root, fix.
            if ((path.Length == 2) && (path[1] == ':') && char.IsLetter(path[0]) && char.IsUpper(path[0]))
                path = path + "\\";

            return path;
        }

        ///
        /// Consts defined in WINBASE.H
        ///
        private enum MoveFileFlags {
            MOVEFILE_REPLACE_EXISTING = 1,
            MOVEFILE_COPY_ALLOWED = 2,
            MOVEFILE_DELAY_UNTIL_REBOOT = 4,
            MOVEFILE_WRITE_THROUGH = 8
        }


        /// <summary>
        /// Marks the file for deletion during next system reboot
        /// </summary>
        /// <param name="lpExistingFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="lpNewFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="dwFlags">MoveFileFlags</param>
        /// <returns>bool</returns>
        /// <remarks>http://msdn.microsoft.com/en-us/library/aa365240(VS.85).aspx</remarks>
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileEx")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        public static FileInfo MarkForDeletion(FileInfo file) {
            MarkForDeletion(file.FullName);
            return file;
        }

        public static string MarkForDeletion(string filename) {
            if (File.Exists(filename) == false)
                return filename;
            //Usage for marking the file to delete on reboot
            MoveFileEx(filename, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
            return filename;
        }

        /// <summary>
        ///     Removes or replaces all illegal characters for path in a string.
        /// </summary>
        public static string RemoveIllegalPathCharacters(string filename, string replacewith = "") => string.Join(replacewith, filename.Split(Path.GetInvalidFileNameChars()));

        public class FilePathEqualityComparer : IEqualityComparer<string> {
            public bool Equals(string x, string y) {
                return Paths.CompareTo(x, y);
            }

            public int GetHashCode(string obj) {
                return Paths.NormalizePath(obj).GetHashCode();
            }
        }
        public class FileInfoPathEqualityComparer : IEqualityComparer<FileSystemInfo> {
            public bool Equals(FileSystemInfo x, FileSystemInfo y) {
                return Paths.CompareTo(x, y);
            }

            public int GetHashCode(FileSystemInfo obj) {
                return Paths.NormalizePath(obj.FullName).GetHashCode();
            }
        }
    }
}