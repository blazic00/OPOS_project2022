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
MyTask task = new MyTask(Algorithm.PixelateImages);
task.Images = images;

taskScheduler.ScheduleTask(task);



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
    taskScheduler.PauseTask(task);
    number = int.Parse(Console.ReadLine());
    taskScheduler.ContinueTask(task);
}


static void Function(SemaphoreSlim semaphore)
{
    for (int i = 0; i < 100000000; i++)
    {
        semaphore.Wait();
        semaphore.Release();
    }
}