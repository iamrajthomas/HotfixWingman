//  -------------------------------------------------------------------------
//  <copyright file="IHotfix.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       IHotfix
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Interface
{
    using System.Collections.Generic;
    public interface IHotfix
    {
        bool Initiate(string SourcePath, string DestinationPath, string BackupPath, bool CreateBackupFlag);
        void CreateBackUp();
        void CleanUpBackupTargetFolder();
        void GetListOfDestinationPathsForEachFileInSourcePath();
        List<string> CreatePathLists();
        bool ReplaceFilesFromSourceToDestination();
        void ReadAllFileNamesFromPath(string SourcePath);
    }
}
