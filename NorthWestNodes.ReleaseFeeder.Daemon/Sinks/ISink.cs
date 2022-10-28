using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Sinks
{
    public interface ISink
    {
        Task Dispatch(params string[] args);
    }
}
