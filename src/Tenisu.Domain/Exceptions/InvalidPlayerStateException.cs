using System;
using System.Collections.Generic;
using System.Text;

namespace Tenisu.Domain.Exceptions
{
    public class InvalidPlayerStateException : Exception
    {
        public InvalidPlayerStateException(string? message) : base(message)
        {
        }
    }
}
