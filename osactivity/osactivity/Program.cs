using System;
using System.Threading;

namespace OS_Problem_02
{
    class Thread_safe_buffer
    {
        static int[] TSBuffer = new int[10];
        static int Front = 0;
        static int Back = 0;
        static int Count = 0;
        static int Endflag = 0;
        static int T1flag = 0;
        static int T11flag = 0;

        private static object _Lock = new object();


        static void EnQueue(int eq)
        {
                while (Count == 10 || (T11flag == 1 && T1flag == 1))
                {
                    Monitor.Wait(_Lock);
                }

                TSBuffer[Back] = eq;
                Back++;
                Back %= 10;
                Count += 1;
                Monitor.Pulse(_Lock);
        }

        static int DeQueue()
        {
            while (Count == 0 ||( T11flag == 1 && T1flag == 1)) {
                lock (_Lock)
                {
                    if (T11flag == 1 && T1flag == 1)
                    {
                        Endflag = 1;
                        Monitor.PulseAll(_Lock);
                        break;
                    }
                    Monitor.Wait(_Lock);
                }
            }
            int x = 0;
            x = TSBuffer[Front];
            Front++;
            Front %= 10;
            Count -= 1;
            return x;
        }

        static void th01()
        {
            int i;

            for (i = 1; i < 51; i++)
            {
                lock (_Lock)
                {
                    EnQueue(i);
                }
                
                Thread.Sleep(5);
            }
            lock (_Lock)
            {
                T1flag = 1;
                Monitor.PulseAll(_Lock);
            }
        }

        static void th011()
        {
            int i;

            for (i = 100; i < 151; i++)
            {
                lock (_Lock)
                {
                    EnQueue(i);
                }
                Thread.Sleep(5);
            }
            lock (_Lock)
            {
                T11flag = 1;
                Monitor.PulseAll(_Lock); 
            }
        }


        static void th02(object t)
        {
            int i;
            int j;

            for (i = 0; i < 60; i++)
            {
                lock (_Lock)
                {
                    j = DeQueue();
                    if (Endflag == 0 )
                    { 
                        Console.WriteLine("j={0}, thread:{1}", j, t);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Thread t1 = new Thread(th01);
            Thread t11 = new Thread(th011);
            Thread t2 = new Thread(th02);
            Thread t21 = new Thread(th02);
            Thread t22 = new Thread(th02);

            t1.Start();
            t11.Start();

            t2.Start(1);
            t21.Start(2);
            t22.Start(3);
            t1.Join();
            t11.Join();
            t2.Join();
            t21.Join();
            t22.Join();
        }
    }
}