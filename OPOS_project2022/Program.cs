// See https://aka.ms/new-console-template for more information


using OPOS_project2022;
using System.Drawing;

MyTaskScheduler taskScheduler = new MyTaskScheduler();
taskScheduler.MaxConcurrentTasks = 3;




var path = MetaData.ProjectPath;
var images1= new List<Bitmap>();
var images2 = new List<Bitmap>();
for (int i = 0; i < 20; i++)
{
    var bmp1 = new Bitmap(path + Path.DirectorySeparatorChar + "koala.jpg");
    images1.Add(bmp1);
    var bmp2 = new Bitmap(path + Path.DirectorySeparatorChar + "koala.jpg");
    images2.Add(bmp2);
}
MyTask task1 = new MyTask(Algorithm.PixelateImages);
task1.Images = images1;
MyTask task2 = new MyTask(Algorithm.PixelateImages);
task2.Images = images2;

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