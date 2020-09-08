using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class MoveResolver
    {
        public ConcurrentBag<MoveResolution> MoveQueue { get; private set; }

        public MoveResolver()
        {
            MoveQueue = new ConcurrentBag<MoveResolution>();
        }

        public void ResolveQueue()
        {

        }
    }
}
