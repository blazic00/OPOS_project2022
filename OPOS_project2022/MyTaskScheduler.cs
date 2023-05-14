using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project2022
{
    internal class MyTaskScheduler {


        //ne razumijem ove getere i setere!
        public int MaxConcurrentTasks{ get; set; } = 1;

        private Queue<MyTask> scheduledTasks = new Queue<MyTask>();
        private HashSet<MyTask> runningTasks = new HashSet<MyTask>();
        private HashSet<MyTask> pausedTasks = new HashSet<MyTask>();


        //public Dictionary<MyTask, List<Bitmap> 

        

        public void ScheduleTask(MyTask task)
        {
            lock (this)
            {
                task.OnJobFinished=HandleJobFinished;
                scheduledTasks.Enqueue(task);
            }
            StartTask();
        }

        public void PauseTask(MyTask task)
        {
            if (runningTasks.Contains(task))
            {
                lock (this)
                {
                    task.SemaphoreSlim.Wait();
                    Console.WriteLine(task.TaskId + " paused!");
                    runningTasks.Remove(task);
                    pausedTasks.Add(task);
                }
                StartTask();
            }
        }
        public void ContinueTask(MyTask task)
        {
            if (pausedTasks.Contains(task))
            {
                lock (this)
                {
                    pausedTasks.Remove(task);
                    scheduledTasks.Enqueue(task);
                    Console.WriteLine(task.TaskId + " unpaused!");
                }
                StartTask();
            }
        }
        public void StartTask()
        {
            try
            {
                lock (this)
                {
                    if (runningTasks.Count < MaxConcurrentTasks)
                    {
                        MyTask task = scheduledTasks.Dequeue();
                        runningTasks.Add(task);
                        if(task.SemaphoreSlim.CurrentCount == 0)
                        {
                            //If task was previously paused
                            task.SemaphoreSlim.Release();
                            Console.WriteLine(task.TaskId + " continues!");
                        }
                        else
                        {
                            //If this task is starting for the first time
                            task.Run();
                        }

                    }
                }

            }
            catch(InvalidOperationException ex)
            {
               //Queue is empty
            }
        }

        private void HandleJobFinished(MyTask job)
        {
            lock (this)
            {
                runningTasks.Remove(job);
                if (scheduledTasks.Count > 0)
                {
                    StartTask();
                }
            }
        }

       
    }
}
