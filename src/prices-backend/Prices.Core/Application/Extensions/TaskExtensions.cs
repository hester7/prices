using System.Runtime.CompilerServices;

namespace Prices.Core.Application.Extensions
{
	public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> AsTask<T>(this T instance) => Task.FromResult(instance);

        // Use this method to return the aggregate exception of all tasks
        public static async Task<IEnumerable<T>> WhenAll<T>(params Task<T>[] tasks)
        {
            // https://www.youtube.com/watch?v=gW19LaAYczI&list=PLUOequmGnXxPjam--7GAls6Tb1fSmL9mL&index=5&ab_channel=NickChapsas

            var allTasks = Task.WhenAll(tasks);

            try
            {
                return await allTasks;
            }
            catch (Exception)
            {
                // ignore
            }

            throw allTasks.Exception!;
        }
    }
}
