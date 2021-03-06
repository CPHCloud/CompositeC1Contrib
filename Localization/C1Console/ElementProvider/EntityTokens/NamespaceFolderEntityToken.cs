﻿using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.Localization.C1Console.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(NamespaceFolderAncestorProvider))]
    public class NamespaceFolderEntityToken : EntityToken
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

        public NamespaceFolderEntityToken(string ns)
        {
            _id = ns;
        }

        public static Element CreateElement(ElementProviderContext context, string label, string ns)
        {
            var dragAndDropInfo = new ElementDragAndDropInfo(typeof(NamespaceFolderEntityToken));

            dragAndDropInfo.AddDropType(typeof(NamespaceFolderEntityToken));
            dragAndDropInfo.AddDropType(typeof(IResourceKey));

            dragAndDropInfo.SupportsIndexedPosition = false;

            var folderHandle = context.CreateElementHandle(new NamespaceFolderEntityToken(ns));
            var folderElement = new Element(folderHandle, dragAndDropInfo)
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

            return new NamespaceFolderEntityToken(id);
        }
    }

    public class NamespaceFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var namespaceToken = entityToken as NamespaceFolderEntityToken;
            if (namespaceToken == null)
            {
                return Enumerable.Empty<EntityToken>();
            }

            var split = namespaceToken.Namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 1)
            {
                return new[] { new LocalizationElementProviderEntityToken() };
            }

            var parentName = String.Join(".", split.Take(split.Length - 1));

            return new[] { new NamespaceFolderEntityToken(parentName) };
        }
    }
}
