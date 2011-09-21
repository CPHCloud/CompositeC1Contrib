﻿using System;
using System.Collections.Generic;
using System.Web;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.Sorting
{
    [ActionExecutor(typeof(UrlActionTokenActionExecutor))]
    internal sealed class UrlActionToken : ActionToken
    {
        private IEnumerable<PermissionType> _permissionTypes;

        public UrlActionToken(string label, string url, IEnumerable<PermissionType> permissionTypes)
        {
            this.Url = url;
            _permissionTypes = permissionTypes;
        }


        public string Label { get; private set; }
        public string Url { get; private set; }


        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }


        public override string Serialize()
        {
            return this.Label + "·" + this.Url + "·" + this.PermissionTypes.SerializePermissionTypes();
        }



        public static ActionToken Deserialize(string serializedData)
        {
            string[] s = serializedData.Split('·');

            return new UrlActionToken(s[0], s[1], s[2].DeserializePermissionTypes());
        }
    }



    internal sealed class UrlActionTokenActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            UrlActionToken urlActionToken = (UrlActionToken)actionToken;

            string currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;

            string url = urlActionToken.Url;
            if (urlActionToken.Url.Contains("?"))
            {
                url = urlActionToken.Url + "&consoleId=" + currentConsoleId;
            }
            else
            {
                url = urlActionToken.Url + "?consoleId=" + currentConsoleId;
            }


            ConsoleMessageQueueFacade.Enqueue(new OpenViewMessageQueueItem { Url = url, ViewId = Guid.NewGuid().ToString(), ViewType = ViewType.Main, Label = urlActionToken.Label }, currentConsoleId);

            return null;
        }
    }
}
