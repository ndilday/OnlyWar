using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class WoundResolver
    {
        public ConcurrentBag<WoundResolution> WoundQueue { get; private set; }

        public WoundResolver()
        {
            WoundQueue = new ConcurrentBag<WoundResolution>();
        }

        public void ResolveQueue()
        {

        }
    }
}
