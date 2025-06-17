using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StressClient {
    public static class CancellationEX {
        
        public static async Task<T> WithCancellation<T> (this Task<T> task, CancellationToken cancellationToken) {

            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(state => {

                ((TaskCompletionSource<object>)state).TrySetResult(null);

            },
            tcs)) {

                var resultTask = await Task.WhenAny(task, tcs.Task);
                if (resultTask == tcs.Task) {
                    throw new OperationCanceledException(cancellationToken);
                }

                return await task;
            }

        }

        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout) {

            using (var cts = new CancellationTokenSource()) {

                var delayTask = Task.Delay(timeout, cts.Token);
                var resultTask = await Task.WhenAny(task, delayTask);

                if (resultTask == delayTask) {
                    throw new OperationCanceledException();
                } else {
                    cts.Cancel();
                }

                return await task;
            }

        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken) {

            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(state => {

                ((TaskCompletionSource)state).TrySetResult();

            },
            tcs)) {

                var resultTask = await Task.WhenAny(task, tcs.Task);
                if (resultTask == tcs.Task) {
                    throw new OperationCanceledException(cancellationToken);
                }

                await task;
            }

        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout) {

            using (var cts = new CancellationTokenSource()) {

                var delayTask = Task.Delay(timeout, cts.Token);
                var resultTask = await Task.WhenAny(task, delayTask);

                if (resultTask == delayTask) {
                    throw new OperationCanceledException();
                } else {
                    cts.Cancel();
                }

                await task;
            }

        }

    }

}
