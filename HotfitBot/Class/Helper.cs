//  -------------------------------------------------------------------------
//  <copyright file="Helper.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       Helper
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot.Class
{
    using HotfitBot.Constants;
    using HotfitBot.Interface;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    class Helper : IHelper
    {
        /// <summary>
        /// InitialValidationForPaths
        /// </summary>
        /// <param name="Paths"></param>
        /// <returns></returns>
        public string InitialValidationForPaths(List<string> Paths)
        {
            string ValidationResult = "";

            if (Paths.Distinct().Count() != Paths.Count())
            {
                ValidationResult = ValidationConstant.Same;
                return ValidationResult;
            }

            foreach (var path in Paths)
            {
                if (path != null && path != "" && Directory.Exists(path))
                {
                    ValidationResult = ValidationConstant.Valid;
                }
                else
                {
                    ValidationResult = ValidationConstant.Invalid;
                    return ValidationResult;
                }
            }

            return ValidationResult;
        }
    }
}
