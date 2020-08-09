using System;

namespace THNETII.GitHubActions.Toolkit.Core
{
    internal class GhLoggingGroupDisposable : IDisposable
    {
        private bool disposed = false;

        public GhLoggingGroupDisposable(string name)
        {
            GhActionsCore.StartGroup(name);
        }

        public void Dispose()
        {
            bool disposed;
            (disposed, this.disposed) = (this.disposed, true);

            if (!disposed)
                GhActionsCore.EndGroup();
        }
    }
}
