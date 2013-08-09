using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskToys
{
    internal class AsyncAutoResetEvent
    {
        private readonly static Task completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> waits = new Queue<TaskCompletionSource<bool>>();
        private volatile bool signaled;

        public Task WaitAsync()
        {
            lock (this.waits)
            {
                if (this.signaled)
                {
                    this.signaled = false;
                    return completed;
                }
                else
                {
                    var source = new TaskCompletionSource<bool>();
                    this.waits.Enqueue(source);
                    return source.Task;
                }
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (this.waits)
            {
                if (this.waits.Count > 0)
                {
                    toRelease = this.waits.Dequeue();
                }
                else if (!this.signaled)
                {
                    this.signaled = true;
                }
            }

            if (toRelease != null)
            {
                toRelease.SetResult(true);
            }
        }
    }
}
