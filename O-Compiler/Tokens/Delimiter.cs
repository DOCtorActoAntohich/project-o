﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace OCompiler.Tokens
{
    sealed class Delimiter : CodeEntity
    {
        public static Delimiter Dot    { get; } = new(".");
        public static Delimiter Comma  { get; } = new(",");
        public static Delimiter Colon  { get; } = new(":");
        public static Delimiter Assign { get; } = new(":=");

        public static Delimiter LeftParenthesis    { get; } = new("(");
        public static Delimiter RightParenthesis   { get; } = new(")");
        public static Delimiter LeftSquareBracket  { get; } = new("[");
        public static Delimiter RightSquareBracket { get; } = new("]");

        private Delimiter(string literal) : base(literal) { }
    }
}
