﻿using System;
using System.Web.Razor.Parser.SyntaxTree;

namespace CompositeC1Contrib.RazorFunctions.Parser
{
    public class TemplateParsingException : Exception
    {
        public int Column { get; private set; }
        public int Line { get; private set; }

        public TemplateParsingException(RazorError error)
            : base(error.Message)
        {
            Column = error.Location.CharacterIndex;
            Line = error.Location.LineIndex;
        }
    }
}
