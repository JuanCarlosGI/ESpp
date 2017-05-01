using System;
using System.Runtime.Serialization;

namespace Coco_R
{
    public class EsppException : Exception
    {
        public EsppException(string message) : base(message)
        {
        }
    }
}