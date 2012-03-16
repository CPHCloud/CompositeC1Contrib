﻿using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.BaseFunctionProviderElementProvider;

namespace CompositeC1Contrib.Security
{
    public class StandardFunctionSecurityAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            string fullname = entityToken.Id;
            string providerName = entityToken.Source;

            if (fullname.Contains('.'))
            {
                fullname = fullname.Remove(fullname.LastIndexOf('.'));
            }

            string id = BaseFunctionProviderElementProvider.CreateId(fullname, "AllFunctionsElementProvider");

            yield return new BaseFunctionFolderElementEntityToken(id);
        }
    }
}
