using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MsTpl
{

        public class Program
        {


            static void Main(string[] args)
            {
                //new Semaphores().RunTests();
                // new EaTest().RunTests();

                //new Delegates().RunTests();

                //HandleTaskGlobalException();
                //RunTask();

                Console.ReadLine();
            }

            private static void HandleTaskGlobalException()
            {
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }


            private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
            {
                e.SetObserved();
            }

            // Task with input Parameters?
            // Using value returned from a task asynchronously without waiting?
            private static void RunTask()
            {
                // Create and start manually
                var t1 = new Task(() => { Console.WriteLine("From within t1"); });
                t1.Start();

                // Create and start automatically
                var t2 = Task.Factory.StartNew(() => { Console.WriteLine("From within t2"); });

                // Create after a task is complete
                var t3 = t2.ContinueWith((prevTask) => { Console.WriteLine("From within t3"); });

                // Task which returns result
                var t4 = Task.Factory.StartNew(() => { return 10; });

                // Task which throws exception
                var t5 = Task.Factory.StartNew(() => { Thread.Sleep(3000); var d = 0; return 10 / d; });

                // Task which defines execution context (not in a console app)
                //var t6 = Task.Factory.StartNew(() => { Console.WriteLine("From within t6 executed in the context of the main method"); }, System.Threading.CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

                // Start execution order
                var t7 = Task.Factory.StartNew(() => { Console.WriteLine("From within t7. T7 STARTS before T8, however, we don't know if it finishes faster or not."); }, System.Threading.CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
                var t8 = Task.Factory.StartNew(() => { Console.WriteLine("From within t8. T8 STARTS after T7, however, we don't know if it finishes faster or not."); }, System.Threading.CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);

                // Start execution order
                var t9 = Task.Factory.StartNew(() => { Console.WriteLine("From within t9. If a task takes too much to complete: LongRunning"); }, System.Threading.CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                // Start execution order
                var cts = new CancellationTokenSource();
                var t10 = Task.Factory.StartNew(() => ParallelForExample(new ParallelOptions { CancellationToken = cts.Token }));
                Thread.Sleep(2000);
                cts.Cancel();

                // Start execution order
                var t11 = Task.Factory.StartNew(SpeculativeExecution);

                // using custom wait logic to handle exception instead of relying on:  Task.WaitAll();
                Console.WriteLine("Waiting for threads...");
                Waitfor(new List<Task> { t1, t2, t3, t4, t5, t7, t8, t9, t10, t11 });

                Console.WriteLine("DONE! Please press a key to exit...");
                Console.ReadKey();
            }


            private static void SpeculativeExecution()
            {
                var urls = new List<string> { "url1", "url2", "url3" };
                var tasks = new List<Task<string>>();

                var cs = new CancellationTokenSource();

                foreach (var url in urls)
                {
                    tasks.Add(Task.Factory.StartNew(() => Download(url), cs.Token));
                }

                var winnerIndex = Task.WaitAny(tasks.ToArray());
                cs.Cancel();

                var downloaded = tasks[winnerIndex].Result;
            }


            private static string Download(string url)
            {
                /* Downloading...*/
                Thread.Sleep(new Random().Next(100, 1000));
                return url;
            }


            private static void ParallelForExample(ParallelOptions parallelOptions)
            {
                try
                {
                    Parallel.For(0, 100000, parallelOptions, (i) => { Console.Write("."); Thread.Sleep(100); });
                }
                catch (OperationCanceledException ce)
                {
                    Console.WriteLine("Parallel.For loop Cancelled...");
                    /* Loop cancelled, ignore exception.*/
                }
            }


            private static void Waitfor(List<Task> tasks)
            {
                while (tasks.Count > 0)
                {
                    var index = Task.WaitAny(tasks.ToArray());

                    if (tasks[index].Exception != null)
                    {
                        // Handle task exception
                        Console.WriteLine("Task throw: {0}", tasks[index].Exception);
                    }
                    else
                    {
                        // Print task result
                        var result = tasks[index] as Task<int>;
                        if (result != null) Console.WriteLine("Task returned {0} as result", result.Result);
                    }

                    tasks.RemoveAt(index);
                }
            }
        }
    }
}
