//  -------------------------------------------------------------------------
//  <copyright file="Hotfix.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       Hotfix
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Class
{
    using HotfitBot.Constants;
    using HotfitBot.Interface;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// HotFix 
    /// </summary>
    public class Hotfix : IHotfix
    {
        private bool IsOperationSucceed { get; set; }
        private readonly ReadAppConfigData _readAppConfigData = null;
        private readonly Logger _logger = null;
        private List<string> ListOfFileNamesPresentInSourcePath = null;
        private Dictionary<string, string> Dict_FileNameWithDestinationPath = null;

        // Application Supporting Variables 
        private static string SourcePathInput = null;
        private static string BackupPathInput = null;
        private static string DestinationPathInput = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public Hotfix()
        {
            _readAppConfigData = new ReadAppConfigData();
            _logger = new Logger();
            IsOperationSucceed = false;
            ListOfFileNamesPresentInSourcePath = new List<string>();
            Dict_FileNameWithDestinationPath = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initiate - Start of the Hotfix
        /// @ToDO: 
        /// 0. Validate if paths are correct and proceed accordingly 
        /// 1. Read the name of the binaries files from source and keep it in a list
        /// 2. For each binaries in source, check the destination and keep the exact locations for each binaries in a list
        /// 3. Take Backup 
        /// 4. Based on the Step2 data, replace the binaries 
        /// 5. Complete the logs/ check for corner cases/ exception handle and user friendly message on UI
        /// </summary>
        public bool Initiate(string SourcePath, string DestinationPath, string BackupPath, bool CreateBackupFlag)
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => Initiate() :: STARTED");

                // Assigning this to make use of these later
                SourcePathInput = SourcePath;
                DestinationPathInput = DestinationPath;
                BackupPathInput = BackupPath; // Remove this later

                Regex reg = new Regex("[*'\",/:_&#^@]");
                string DateTimeCustomFormat = _readAppConfigData.ReadValueByKey("DateTimeCustomFormat") != null ?
                    _readAppConfigData.ReadValueByKey("DateTimeCustomFormat") : DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                BackupPathInput = BackupPath + @"\" + reg.Replace(DateTime.Now.ToString(DateTimeCustomFormat), " ");

                // Step-0: Validate if paths are correct and proceed accordingly
                List<string> Paths = CreatePathLists();
                string ValidationResult = new Helper().InitialValidationForPaths(Paths);

                switch (ValidationResult)
                {
                   case ValidationConstant.Same: 
                        _logger.PrintDebugLogs($"HotFix => Initiate() :: Initial Validation for Source/ Destination is Failed :: Source/ Destination are Same. Please provide correct data.");
                        MessageBox.Show("Source/ Destination are Same. Please provide correct data.", "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    case ValidationConstant.Invalid:
                        _logger.PrintDebugLogs($"HotFix => Initiate() :: Initial Validation for Source/ Destination is Failed :: Source/ Destination combinations are Invalid. Please provide correct data");
                        MessageBox.Show("Source/ Destination combinations are Invalid. Please provide correct data.", "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //MessageBox.Show("Ullu Bana Rahe Ho!! Pehle Correct Locations Pass Toh Karo!!", "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    case ValidationConstant.Valid:
                        _logger.PrintDebugLogs($"HotFix => Initiate() :: Initial Validation for Source/ Destination is Successful.");
                        break;
                    default:
                        break;
                }

                // Step-1: Read the name of the binaries files from source and keep it in a list
                ReadAllFileNamesFromPath(SourcePath);

                // Step-2: For each binaries in source, check the destination and keep the exact locations for each binaries in a list
                GetListOfDestinationPathsForEachFileInSourcePath();

                if (CreateBackupFlag)
                {
                    // Step-3: Take Backup 
                    CreateBackUp();
                }

                // Step-4: Based on the Step2 data, replace the binaries 
                IsOperationSucceed = ReplaceFilesFromSourceToDestination();
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => Initiate() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
            _logger.PrintDebugLogs($"HotFix => Initiate() :: ENDED");
            return IsOperationSucceed;
        }

        /// <summary>
        /// Create BackUp 
        /// This method keeps a backup of all the destination files, which will be replaced after the patch
        /// </summary>
        public void CreateBackUp()
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: STARTED");
                
                // Determine whether the backup directory exists.
                if (Directory.Exists(BackupPathInput))
                {
                    _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: The backup directory exists. No need to Craete one.");
                }
                else
                {
                    _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: That backup directory does not exist. Create one new backup directory.");
                    DirectoryInfo di = Directory.CreateDirectory(BackupPathInput);
                    _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: The backup directory \"[{Path.GetFileName(BackupPathInput)}]\" is created successfully at {Directory.GetCreationTime(BackupPathInput)}.");
                }

                CleanUpBackupTargetFolder();

                // =========================================================================
                // Copy All Binaries At Flat Root Location
                string backupSourcePath = DestinationPathInput;
                string backupTargetPath = BackupPathInput;
                foreach (var fileWithDestination in Dict_FileNameWithDestinationPath)
                {
                    var filePathWithDestination = fileWithDestination.Value.Split(new[] { "||" }, StringSplitOptions.None);
                    var DestinationFilenameWithPath = Path.Combine(filePathWithDestination[0], fileWithDestination.Key);
                    var BackupFileNameWithPath = Path.Combine(backupTargetPath, fileWithDestination.Key);
                    _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: Taking Backup of the FileName: \"[{DestinationFilenameWithPath}]\"");

                    File.Copy(DestinationFilenameWithPath, BackupFileNameWithPath);
                }

                // =========================================================================
                // Copy All Binaries At Specific Folder Structured Location
                // @ToDo: Implement Later Though if required
                //string backupSourcePath = DestinationPathInput;
                //string backupTargetPath = BackupPathInput;
                //foreach (var fileWithDestination in Dict_FileNameWithDestinationPath)
                //{
                //    var filePathWithDestination = fileWithDestination.Value.Split(new[] { "||" }, StringSplitOptions.None);

                //    foreach (var filePath in filePathWithDestination)
                //    {
                //        var SourceWithPath = ListOfFileNamesPresentInSourcePath.Where(x => x.Contains(fileWithDestination.Key)).FirstOrDefault();
                //        var DestinationFilenameWithPath = Path.Combine(filePath, fileWithDestination.Key);
                //        var BackupFileNameWithPath = Path.Combine(backupTargetPath, fileWithDestination.Key);
                //        if (File.Exists(BackupFileNameWithPath))
                //        {
                //            File.Delete(BackupFileNameWithPath);
                //        }
                //        File.Copy(DestinationFilenameWithPath, BackupFileNameWithPath);
                //    }
                //}

                _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: The backup is made under path: " + backupTargetPath);
                _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: STARTED");

            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => CreateBackUp() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
        }

        /// <summary>
        /// Clean-Up Backup Target Folder
        /// </summary>
        public void CleanUpBackupTargetFolder()
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => CleanUpBackupTargetFolder() :: STARTED");

                foreach (var SourceFileNameWithPath in ListOfFileNamesPresentInSourcePath)
                {
                    string OnlySourceFileName = Path.GetFileName(SourceFileNameWithPath);
                    var filenameWithPath = Path.Combine(BackupPathInput, OnlySourceFileName);
                    if (File.Exists(filenameWithPath))
                    {
                        _logger.PrintDebugLogs($"HotFix => CleanUpBackupTargetFolder() :: The backup target file already exists, so deleting it.");
                        File.Delete(filenameWithPath);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => CleanUpBackupTargetFolder() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
            _logger.PrintDebugLogs($"HotFix => CleanUpBackupTargetFolder() :: ENDED");

        }

        /// <summary>
        /// Get List Of Destination Paths For Each File In Source Path
        /// This method checks for all the destination paths and keep tracks of it, for every source files we need to patch
        /// </summary>
        public void GetListOfDestinationPathsForEachFileInSourcePath()
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => GetListOfDestinationPathsForEachFileInSourcePath() :: STARTED");
                //fetch all directories
                var AllDestinationDirectoriesAndSubDirectories = Directory.GetDirectories(DestinationPathInput, "*", SearchOption.AllDirectories);

                List<string> List_OnlySourceFileName = new List<string>();
                foreach (var SourceFileNameWithPath in ListOfFileNamesPresentInSourcePath)
                {
                    string OnlySourceFileName = Path.GetFileName(SourceFileNameWithPath);
                    List_OnlySourceFileName.Add(OnlySourceFileName);
                }
                List_OnlySourceFileName = List_OnlySourceFileName.Distinct().ToList();

                foreach (var destinationDir in AllDestinationDirectoriesAndSubDirectories)
                {
                    foreach (var OnlySourceFileName in List_OnlySourceFileName)
                    {
                        var searchFileNameWithDestinationPath = Path.Combine(destinationDir, OnlySourceFileName);
                        if (File.Exists(searchFileNameWithDestinationPath))
                        {
                            if (Dict_FileNameWithDestinationPath.Keys.Contains(OnlySourceFileName))
                            {
                                Dict_FileNameWithDestinationPath[OnlySourceFileName] = Dict_FileNameWithDestinationPath[OnlySourceFileName] + "||" + destinationDir;
                            }
                            else
                            {
                                Dict_FileNameWithDestinationPath.Add(OnlySourceFileName, destinationDir);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => GetListOfDestinationPathsForEachFileInSourcePath() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
            _logger.PrintDebugLogs($"HotFix => GetListOfDestinationPathsForEachFileInSourcePath() :: ENDED");
        }

        /// <summary>
        /// Create Path Lists
        /// This is just a helper class to work with others easily
        /// </summary>
        /// <returns></returns>
        public List<string> CreatePathLists()
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => CreatePathLists() :: STARTED");
                List<string> Paths = new List<string>()
                {
                    SourcePathInput,
                    DestinationPathInput
                };

                _logger.PrintDebugLogs($"HotFix => CreatePathLists() :: ENDED");
                return Paths;
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => CreatePathLists() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
        }

        /// <summary>
        /// Replace Files From Source To Destination
        /// </summary>
        public bool ReplaceFilesFromSourceToDestination()
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => ReplaceFilesFromSourceToDestination() :: STARTED");
                foreach (var fileWithDestination in Dict_FileNameWithDestinationPath)
                {
                    var filePathWithDestination = fileWithDestination.Value.Split(new[] { "||" }, StringSplitOptions.None);

                    foreach (var filePath in filePathWithDestination)
                    {
                        var SourceWithPath = ListOfFileNamesPresentInSourcePath.Where(x => x.Contains(fileWithDestination.Key)).FirstOrDefault();
                        var DestinationFilenameWithPath = Path.Combine(filePath, fileWithDestination.Key);
                        if (File.Exists(DestinationFilenameWithPath))
                        {
                            File.Delete(DestinationFilenameWithPath);
                        }
                        _logger.PrintDebugLogs($"HotFix => ReplaceFilesFromSourceToDestination() :: Source FileName: \"[{SourceWithPath}]\" :: Destination FileName: \"[{DestinationFilenameWithPath}]\"");
                        File.Copy(SourceWithPath, DestinationFilenameWithPath);
                    }
                }
                _logger.PrintDebugLogs($"HotFix => ReplaceFilesFromSourceToDestination() :: ENDED");

                return true;
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => ReplaceFilesFromSourceToDestination() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                return false;
            }
        }

        /// <summary>
        /// Read All File Names From Path
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <returns></returns>
        public void ReadAllFileNamesFromPath(string SourcePath)
        {
            try
            {
                _logger.PrintDebugLogs($"HotFix => ReadAllFileNamesFromPath() :: STARTED");

                var HotFixBackupFolderName = _readAppConfigData.ReadValueByKey("HotFixBackupFolderName") != null ?
                   _readAppConfigData.ReadValueByKey("HotFixBackupFolderName") : HotfixConstant.DefaultHotFixBackupFolderName;

                // ListOfFileNamesPresentInSourcePath = Directory.GetFiles(SourcePath, "*.dll").ToList();
                // ListOfFileNamesPresentInSourcePath = Directory.GetFiles(SourcePath).ToList();
                ListOfFileNamesPresentInSourcePath = Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories).ToList();
                ListOfFileNamesPresentInSourcePath = ListOfFileNamesPresentInSourcePath.Where(x => !x.Contains(HotFixBackupFolderName)).ToList(); //THIS IS TO NOT INCLUDE THE BACKUP PATCH DLLS IF PRESENT
                _logger.PrintDebugLogs($"HotFix => ReadAllFileNamesFromPath() :: ENDED");
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"HotFix => ReadAllFileNamesFromPath() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                throw;
            }
        }
    }
}
