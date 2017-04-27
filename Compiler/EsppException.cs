using System;

namespace Compiler
{
    /// <summary>
    /// En exception thrown by the compiler.
    /// </summary>
    internal class EsppException : Exception
    {
        public EsppException(string text) : base(text) { }
    }
}
