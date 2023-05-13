// See https://aka.ms/new-console-template for more information

//Task.Run(() => Console.WriteLine("Blabla"));



using OPOS_project2022;

MyTaskScheduler taskScheduler = new MyTaskScheduler();
taskScheduler.maxConcurrentTasks = 3;

var tasks = new List<MyTask>();
for (int i = 0; i < 6; i++)
{
    tasks.Add(new MyTask(Function));
}

foreach(var task in tasks)
{
    taskScheduler.ScheduleTask(task);
}

while (true)
{
    int number1 = int.Parse(Console.ReadLine());
    taskScheduler.PauseTask(tasks[number1]);
    int number2 = int.Parse(Console.ReadLine());
    taskScheduler.PauseTask(tasks[number2]);

    number1 = int.Parse(Console.ReadLine());
    taskScheduler.ContinueTask(tasks[number1]);

    number2 = int.Parse(Console.ReadLine());
    taskScheduler.ContinueTask(tasks[number2]);
}


static void Function(SemaphoreSlim semaphore)
{
    for (int i = 0; i < 100000000; i++)
    {
        semaphore.Wait();
        semaphore.Release();
    }
}