//  -------------------------------------------------------------------------
//  <copyright file="IReadAppConfigData.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       IReadAppConfigData
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Interface
{
    public interface IReadAppConfigData
    {
        string ReadValueByKey(string Key);
    }
}
