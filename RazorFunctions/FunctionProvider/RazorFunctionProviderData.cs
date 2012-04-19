﻿using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;

using Composite.Functions.Plugins.FunctionProvider;

namespace CompositeC1Contrib.RazorFunctions.FunctionProvider
{
    [Assembler(typeof(RazorFunctionProviderAssembler))]
    public class RazorFunctionProviderData : FunctionProviderData
    {
        [ConfigurationProperty("directory", IsRequired = false, DefaultValue = "~/App_Data/Razor")]
        public string Directory
        {
            get { return (string)base["directory"]; }
            set { base["directory"] = value; }
        }
    }
}
