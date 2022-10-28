using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Feeds
{
    public interface IFeed
    {
        Task Execute();
        bool IsRunning();
    }
}
