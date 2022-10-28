using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Sinks
{
    public abstract class BaseSink : ISink
    {
        public abstract Task Dispatch(params string[] args);
    }
}
