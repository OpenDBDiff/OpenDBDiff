﻿using System;
using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionComparison : Schema.Model.IOptionComparison
    {

        public enum CaseSensityOptions
        {
            Automatic = 0,
            CaseInsensity = 1,
            CaseSensity = 2
        }

        public SqlOptionComparison()
        {
            CaseSensityInCode = CaseSensityOptions.CaseInsensity;
            IgnoreWhiteSpacesInCode = false;
        }

        public SqlOptionComparison(IOptionComparison comparision)
        {
            this.ReloadComparisonOnUpdate = comparision.ReloadComparisonOnUpdate;
            var options = comparision.GetOptions();
            IgnoreWhiteSpacesInCode = bool.Parse(options["IgnoreWhiteSpacesInCode"]);
            CaseSensityInCode = (CaseSensityOptions)Enum.Parse(typeof(CaseSensityOptions), options["CaseSensityInCode"], true);
            CaseSensityType = (CaseSensityOptions)Enum.Parse(typeof(CaseSensityOptions), options["CaseSensityType"], true);
        }

        public bool IgnoreWhiteSpacesInCode { get; set; }
        public bool ReloadComparisonOnUpdate { get; set; }


        public CaseSensityOptions CaseSensityInCode { get; set; }

        public CaseSensityOptions CaseSensityType { get; set; }

        public IDictionary<string, string> GetOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("IgnoreWhiteSpacesInCode", IgnoreWhiteSpacesInCode.ToString());
            options.Add("ReloadComparisonOnUpdate", ReloadComparisonOnUpdate.ToString());
            options.Add("CaseSensityInCode", CaseSensityInCode.ToString());
            options.Add("CaseSensityType", CaseSensityType.ToString());
            return options;
        }
    }
}
