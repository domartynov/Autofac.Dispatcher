using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autofac.Dispatcher
{
    public struct AggregateResultTask
    {
        private Task _task1;
        private List<Task> _tasks;

        public void Add(Task task)
        {
            if (task == null || task.IsCompleted) return;

            if (_task1 == null)
            {
                _task1 = task;
            }
            else if (_tasks == null)
            {
                _tasks = new List<Task> {_task1, task};
            }
            else
            {
                _tasks.Add(task);
            }
        }

        public Task Task
        {
            get
            {
                return _tasks != null
                    ? Task.WhenAll(_tasks)
                    : (_task1 ?? Task.FromResult(true));
            }
        }
    }
}