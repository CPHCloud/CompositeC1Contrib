﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Composite.C1Console.Security;
using Composite.Functions;

using CompositeC1Contrib.Security;

namespace CompositeC1Contrib.FunctionProvider
{
    public abstract class FileBasedFunction<T> : IFunction where T : FileBasedFunction<T>
    {
        private FileBasedFunctionProvider<T> _provider;

        protected string VirtualPath { get; private set; }
        protected IDictionary<string, FunctionParameterHolder> Parameters { get; private set; }

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public Type ReturnType { get; private set; }
        public string Description { get; private set; }

        public EntityToken EntityToken
        {
            get { return new FileBasedFunctionEntityToken(_provider.GetType().Name, String.Join(".", Namespace, Name)); }
        }

        public virtual IEnumerable<ParameterProfile> ParameterProfiles
        {
            get
            {
                if (Parameters != null)
                {
                    foreach (var param in Parameters.Values)
                    {
                        BaseValueProvider defaultValueProvider = new NoValueValueProvider();
                        WidgetFunctionProvider widgetProvider = null;
                        var label = param.Name;
                        var isRequired = true;
                        var helpText = String.Empty;

                        if (param.Attribute != null)
                        {
                            label = param.Attribute.Label;
                            helpText = param.Attribute.HelpText;

                            isRequired = !param.Attribute.HasDefaultValue;
                            if (!isRequired)
                            {
                                defaultValueProvider = new ConstantValueProvider(param.Attribute.DefaultValue);
                            }

                            if (!String.IsNullOrEmpty(param.Attribute.WidgetMarkup))
                            {
                                var el = XElement.Parse(param.Attribute.WidgetMarkup);

                                widgetProvider = new WidgetFunctionProvider(el);
                            }
                        }

                        if (widgetProvider == null)
                        {
                            widgetProvider = StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(param.Type);
                        }

                        yield return new ParameterProfile(param.Name, param.Type, isRequired, defaultValueProvider, widgetProvider, label, new HelpDefinition(helpText));
                    }
                }
            }
        }

        public FileBasedFunction(string ns, string name, string description, IDictionary<string, FunctionParameterHolder> parameters, Type returnType, string virtualPath, FileBasedFunctionProvider<T> provider)
        {
            _provider = provider;

            Namespace = ns;
            Name = name;
            Description = description;
            Parameters = parameters;
            ReturnType = returnType;
            VirtualPath = virtualPath;
        }

        public abstract object Execute(ParameterList parameters, FunctionContextContainer context);
    }
}
