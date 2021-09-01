﻿/*
 * Copyright (c) 2020 Proton Technologies AG
 *
 * This file is part of ProtonVPN.
 *
 * ProtonVPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ProtonVPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ProtonVPN.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtonVPN.Common.Extensions;

namespace ProtonVPN.Common.Logging
{
    public class LogCleaner
    {
        private readonly ILogger _logger;

        public LogCleaner(ILogger logger)
        {
            _logger = logger;
        }

        public void Clean(string logPath, int maxFiles)
        {
            _logger.Info($"[LogCleaner] Checking for log files to be deleted in folder '{logPath}'. The maximum number of files allowed is {maxFiles}.");

            IList<FileInfo> files = GetFiles(logPath).ToList();
            _logger.Debug($"[LogCleaner] The folder '{logPath}' has {files.Count} files.");

            IList<FileInfo> filesToDelete = GetFilesToDelete(files, maxFiles);
            DeleteFiles(filesToDelete);
        }

        private IEnumerable<FileInfo> GetFiles(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                return directoryInfo.GetFiles();
            }
            catch (Exception e) when (e.IsFileAccessException())
            {
                _logger.Error($"[LogCleaner] An error occurred when reading the files from the log folder '{path}'.", e);
                return new FileInfo[0];
            }
        }

        private IList<FileInfo> GetFilesToDelete(IList<FileInfo> files, int maxFiles)
        {
            IList<FileInfo> filesExceedingLimit = GetFilesExceddingLimit(files, maxFiles);
            IList<FileInfo> oldFiles = GetOldFiles(files);

            IList<FileInfo> filesToDelete = filesExceedingLimit.Concat(oldFiles).Distinct().ToList();
            string fileNamesToDelete = GetFileNames(filesToDelete);
            _logger.Info($"[LogCleaner] The folder has {filesToDelete.Count} files to delete.{fileNamesToDelete}");

            return filesToDelete;
        }

        private IList<FileInfo> GetFilesExceddingLimit(IList<FileInfo> files, int maxFiles)
        {
            IList<FileInfo> filesExceedingLimit = files
                .OrderByDescending(LastWriteTime)
                .Skip(maxFiles)
                .ToList();
            string fileNamesExceedingLimit = GetFileNames(filesExceedingLimit);
            _logger.Debug($"[LogCleaner] The folder has {filesExceedingLimit.Count} files exceeding the limit of {maxFiles} files.{fileNamesExceedingLimit}");
            return filesExceedingLimit;
        }

        private DateTime LastWriteTime(FileInfo file)
        {
            try
            {
                return file.LastWriteTimeUtc;
            }
            catch (Exception e) when (e.IsFileAccessException() || e is ArgumentOutOfRangeException)
            {
                _logger.Error(e.Message);
                return DateTime.MaxValue;
            }
        }

        private string GetFileNames(IList<FileInfo> files)
        {
            string fileNames = string.Empty;
            if (files.Any())
            {
                fileNames = $" File names: {string.Join(",", files.Select(fi => fi.Name))}";
            }
            return fileNames;
        }

        private IList<FileInfo> GetOldFiles(IList<FileInfo> files)
        {
            DateTime minimumDate = DateTime.UtcNow.AddMonths(-1);
            IList<FileInfo> oldFiles = files.Where(t => LastWriteTime(t) < minimumDate).ToList();
            string oldFileNames = GetFileNames(oldFiles);
            _logger.Debug($"[LogCleaner] The folder has {oldFiles.Count} old files with a last write date before {minimumDate:O}.{oldFileNames}");
            return oldFiles;
        }

        private void DeleteFiles(IList<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                DeleteFile(file.FullName);
            }
        }

        private void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                _logger.Info($"[LogCleaner] Successfully deleted the file '{fileName}'.");
            }
            catch (Exception e) when (e.IsFileAccessException())
            {
                _logger.Error($"[LogCleaner] An error occurred when deleting the file '{fileName}'.", e);
            }
        }
    }
}
