﻿using System;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.UserControlFunctions.Security
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public class UserControlFunctionEntityToken : EntityToken
    {
        private string _id;
        public override string Id
        {
            get { return this._id; ; }
        }

        private string _source;
        public override string Source
        {
            get { return this._source; }
        }

        public override string Type
        {
            get { return String.Empty; ; }
        }

        public UserControlFunctionEntityToken(string source, string id)
        {
            _source = source;
            _id = id;
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

            EntityToken.DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new UserControlFunctionEntityToken(source, id);
        }
    }
}