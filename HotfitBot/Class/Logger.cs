//  -------------------------------------------------------------------------
//  <copyright file="Logger.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       Logger
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Class
{
    using HotfitBot.Constants;
    using HotfitBot.Interface;
    using System;
    public class Logger : ILogger
    {
        private readonly ReadAppConfigData _readAppConfigData = null;
        private readonly string LoggerFilePath = string.Empty; 

        public Logger()
        {
            _readAppConfigData = new ReadAppConfigData();
            LoggerFilePath = _readAppConfigData.ReadValueByKey("LoggerFilePath") != null ? _readAppConfigData.ReadValueByKey("LoggerFilePath") : HotfixConstant.DefaultLoggerFilePath;
        }

        /// <summary>
        /// PrintDebugLogs
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="LineNumber"></param>
        /// <param name="CallerMemberName"></param>
        /// <param name="CalledFilePath"></param>
        /// <param name="IsPrintStackTrace"></param>
        public void PrintDebugLogs(
            string Message,
            [System.Runtime.CompilerServices.CallerLineNumber] int LineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string CallerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string CalledFilePath = null,
            bool IsPrintStackTrace = false)
        {
            try
            {
                string MessageToBePrinted = string.Empty;
                string Star = string.Format("{0}******************************************************************************************************{0}", System.Environment.NewLine);

                MessageToBePrinted = string.Format("{1} [TimeStamp]: {2}{0} [Message]: {3},{0} [At Line Number]: {4},{0} [CallerMemberName]: {5},{0} [CalledFilePath]: {6},{0} {7}{0}",
                                                    Environment.NewLine,
                                                    Star,
                                                    DateTime.Now.ToString(),
                                                    Message ?? "<<< The Passed Value As Message Is Null >>>",
                                                    LineNumber,
                                                    CallerMemberName,
                                                    CalledFilePath,
                                                    IsPrintStackTrace ? string.Format("[StackTrace]: {0}", Environment.StackTrace) : string.Empty);

                System.IO.File.AppendAllText(LoggerFilePath, MessageToBePrinted);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
