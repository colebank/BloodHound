using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;

namespace ThreadedRole
{
    public abstract class ThreadedRoleEntryPoint : RoleEntryPoint
    {
        private List<Thread> Threads = new List<Thread>();
        private WorkerEntryPoint[] Workers;
        protected EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public override void Run()
        {
            foreach (WorkerEntryPoint worker in Workers)
                Threads.Add(new Thread(worker.ProtectedRun));

            foreach (Thread thread in Threads)
                thread.Start();

            while (!EventWaitHandle.WaitOne(0))
            {
                // WWB: Restart Dead Threads
                for (Int32 i = 0; i < Threads.Count; i++)
                {
                    if (!Threads[i].IsAlive)
                    {
                        Threads[i] = new Thread(Workers[i].ProtectedRun);//Run);
                        Threads[i].Start();
                    }
                }

                EventWaitHandle.WaitOne(1000);
            }

        }

        public bool OnStart(WorkerEntryPoint[] workers)
        {
            this.Workers = workers;

            foreach (WorkerEntryPoint worker in workers)
                worker.OnStart();

            return base.OnStart();
        }

        public override bool OnStart()
        {
            throw (new InvalidOperationException());
        }

        public override void OnStop()
        {
            EventWaitHandle.Set();

            foreach (Thread thread in Threads)
                while (thread.IsAlive)
                    thread.Abort();

            // WWB: Check To Make Sure The Threads Are
            // Not Running Before Continuing
            foreach (Thread thread in Threads)
                while (thread.IsAlive)
                    Thread.Sleep(10);

            // WWB: Tell The Workers To Stop Looping
            foreach (WorkerEntryPoint worker in Workers)
                worker.OnStop();

            base.OnStop();
        }
    }



    public class WorkerEntryPoint
    {
        public virtual bool OnStart()
        {
            return (true);
        }

        /// <summary>
        /// This method prevents unhandled exceptions from being thrown
        /// from the worker thread.
        /// </summary>
        internal void ProtectedRun()
        {
            try
            {
                // Call the Workers Run() method
                Run();
            }
            catch (SystemException)
            {
                // Exit Quickly on a System Exception
                throw;
            }
            catch (Exception)
            {
            }
        }

        public virtual void Run()
        {
        }

        public virtual void OnStop()
        {
        }
    }
}
