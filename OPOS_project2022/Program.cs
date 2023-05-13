// See https://aka.ms/new-console-template for more information


using OPOS_project2022;
using System.Drawing;

MyTaskScheduler taskScheduler = new MyTaskScheduler();
taskScheduler.maxConcurrentTasks = 3;




var path = MetaData.ProjectPath;
var images = new List<Bitmap>();
for(int i = 0; i < 7; i++)
{
    var bmp = new Bitmap(path + Path.DirectorySeparatorChar + "koala.jpg");
    images.Add(bmp);
}
MyTask task1 = new MyTask(Algorithm.PixelateImages);
task1.Images = images;
MyTask task2 = new MyTask(Algorithm.PixelateImages);
task2.Images = images;

taskScheduler.ScheduleTask(task1);
taskScheduler.ScheduleTask(task2);



/*var tasks = new List<MyTask>();
for (int i = 0; i < 6; i++)
{
    tasks.Add(new MyTask(Function));
}

foreach(var task in tasks)
{
    taskScheduler.ScheduleTask(task);
}
*/
while (true)
{
    int number = int.Parse(Console.ReadLine());
    taskScheduler.PauseTask(task1);
    number = int.Parse(Console.ReadLine());
    taskScheduler.ContinueTask(task1);
}


static void Function(SemaphoreSlim semaphore)
{
    for (int i = 0; i < 100000000; i++)
    {
        semaphore.Wait();
        semaphore.Release();
    }
}