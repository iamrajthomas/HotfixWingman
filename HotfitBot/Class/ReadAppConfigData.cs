//  -------------------------------------------------------------------------
//  <copyright file="ReadAppConfigData.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       ReadAppConfigData
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Class
{
    using HotfitBot.Interface;
    using System.Configuration;
    public class ReadAppConfigData : IReadAppConfigData
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public ReadAppConfigData()
        {

        }

        /// <summary>
        /// Read App Config Key Value by Key
        /// </summary>
        public string ReadValueByKey(string Key)
        {
            string Value = ConfigurationManager.AppSettings.Get(Key);
            return Value;
        }
    }
}
