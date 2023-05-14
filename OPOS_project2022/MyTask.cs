using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project2022
{
    internal class MyTask
    {
        private static int taskIds = 0;
       
        public int TaskId { get; set; }
        public SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1);

        public List<Bitmap> Images { get; set; }


        private Task task;
        public Action<MyTask> OnJobFinished { set; get; }

        public MyTask(Action<SemaphoreSlim, List<Bitmap>,int,bool,int> taskAction)
        {
            TaskId = taskIds++;
            task = new Task(() =>
            {
                try
                {
                    Console.WriteLine(TaskId + " started!");
                    //Hardkodovano!
                    taskAction.Invoke(SemaphoreSlim,Images,1,false,this.TaskId);
                }
                finally
                {
                    Console.WriteLine(TaskId + " finished!");
                    this.OnJobFinished.Invoke(this);
                }
            });
            /*this.onJobFinished = onJobFinished;*/
        }

        public void Run()
        {
            task.Start();
        }
    }
}
