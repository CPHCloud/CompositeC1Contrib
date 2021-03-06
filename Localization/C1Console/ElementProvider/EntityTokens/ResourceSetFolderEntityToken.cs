﻿using System;
using System.Collections.Generic;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.Localization.C1Console.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(ResourceSetFolderAncestorProvider))]
    public class ResourceSetFolderEntityToken : EntityToken
    {
        public override string Type
        {
            get { return String.Empty; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public string Namespace
        {
            get { return _id; }
        }

        public ResourceSetFolderEntityToken(string resourceSet)
        {
            _id = resourceSet;
        }

        public static Element CreateElement(ElementProviderContext context, string label, string ns)
        {
            var folderHandle = context.CreateElementHandle(new NamespaceFolderEntityToken(ns));
            var folderElement = new Element(folderHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = label,
                    ToolTip = label,
                    HasChildren = true,
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("datagroupinghelper-folder-closed"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("datagroupinghelper-folder-open")
                }
            };

            return folderElement;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new ResourceSetFolderEntityToken(id);
        }
    }

    public class ResourceSetFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            return new[] { new LocalizationElementProviderEntityToken() };
        }
    }
}
