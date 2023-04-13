using System;

namespace EVRC.Core
{
    public class HudColorMatrixSyntaxErrorException : Exception
    {
        public HudColorMatrixSyntaxErrorException() { }
        public HudColorMatrixSyntaxErrorException(string message) : base(message) { }
        public HudColorMatrixSyntaxErrorException(string message, Exception inner) : base(message, inner) { }
    }
}