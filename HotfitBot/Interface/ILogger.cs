//  -------------------------------------------------------------------------
//  <copyright file="ILogger.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       ILogger
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Interface
{
    public interface ILogger
    {
        void PrintDebugLogs(
            string Message,
            [System.Runtime.CompilerServices.CallerLineNumber] int LineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string CallerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string CalledFilePath = null,
            bool IsPrintStackTrace = false);
    }
}
