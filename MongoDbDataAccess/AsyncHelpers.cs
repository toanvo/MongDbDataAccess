using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbDataAccess
{
    internal class AsyncHelpers
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
