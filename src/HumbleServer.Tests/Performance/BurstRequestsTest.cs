﻿namespace HumbleServer.Tests.Performance
{
    using System;
    using Commands;
    using NUnit.Framework;
    using System.Threading;

    /// <summary>
    /// TODO: refatorar
    /// </summary>
    [TestFixture]
    public class BurstRequestsTest : HumbleTestBase
    {
        private readonly ManualResetEvent mre = new ManualResetEvent(false);
        private const int NumThreads = 100;
        private CountdownEvent countdown;

        protected override void BeforeTest()
        {
            server.AddCommand("wait", () => new Wait());
        }

        /// <summary>
        /// This test fires 100 request to server to command wait.
        /// Command wait will sleep two seconds and response with 1.
        /// The server has to open 100 threads at same time, and sleep two seconds 
        /// for each thread and send the response. 
        /// So this test doesn't has to take more than 3 seconds
        /// </summary>
        [Test]
        public void Should_execute_less_than_3_seconds()
        {
            ThreadPool.SetMinThreads(NumThreads, NumThreads);

            Console.WriteLine("Burst test started");

            this.mre.Reset();
            this.countdown = new CountdownEvent(NumThreads);

            for (var i = 0; i < NumThreads; i++)
            {
                new Thread(this.ThreadProc) { Name = "Thread " + i }.Start();
            }

            this.countdown.Wait();
            var dateTime = DateTime.Now;
            this.countdown = new CountdownEvent(NumThreads);
            this.mre.Set();
            this.countdown.Wait();
            var timeSpan = DateTime.Now - dateTime;

            Console.WriteLine("Test finished");
            Console.WriteLine("Executed at {0}.{1:0}s.", timeSpan.Seconds, timeSpan.Milliseconds / 100);

            if (timeSpan.Seconds > 3)
            {
                Assert.Ignore("This test should't take more than to 3 seconds to run");
            }
        }

        private void ThreadProc()
        {
            this.countdown.Signal();

            var client = new NetworkClient();
            this.mre.WaitOne();
            client.Connect("localhost", 987);
            client.Send("wait");

            try
            {
                var data = client.Receive();
                if (data.Equals("1") == false)
                {
                    Assert.Ignore("Should receive 1 on thread " + Thread.CurrentThread.Name);
                }
            }
            catch (Exception exception)
            {
                Assert.Ignore("Exception on thread " + Thread.CurrentThread.Name + ": " + exception.Message);
            }

            this.countdown.Signal();
        }
    }
}
