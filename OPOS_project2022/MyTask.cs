﻿using System;
using System.Collections.Generic;
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


       /* private enum JobState
        {
            NotStarted,
            Running,
            Finished,
            Paused
        }
        private JobState jobState=JobState.NotStarted;*/

        private Task task;
        public Action<MyTask> OnJobFinished { set; get; }

        public MyTask(Action<SemaphoreSlim> taskAction)
        {
            TaskId = taskIds++;
            task = new Task(() =>
            {
                try
                {
                    Console.WriteLine(TaskId + " started!");
                    taskAction.Invoke(SemaphoreSlim);
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