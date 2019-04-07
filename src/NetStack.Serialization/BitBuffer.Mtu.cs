using System;
using System.Collections.Generic;
using System.Text;

namespace NetStack.Serialization
{
    ///<summary>
    /// Constants to help proper split messages into fragments.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Maximum_transmission_unit"/>
    /// <seealso href="https://tools.ietf.org/html/rfc5389/"/>
    /// <seealso href="https://tools.ietf.org/html/rfc1191"/>
    partial class BitBuffer
    {
        public const int MtuIeee802Dot3 = 1492;

        public const int MtuIeee802 = 508;
    }
}
