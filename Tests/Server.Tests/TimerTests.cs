using System;
using System.Threading;
using Server;

namespace Server.Tests;

public class TimerTests
{
    [OneTimeSetUp]
    public void StartTimerThread()
    {
        var tt = new Timer.TimerThread();
        var thread = new Thread(tt.TimerMain)
        {
            IsBackground = true
        };
        thread.Start();
    }

    private static void ProcessTimers(int durationMs)
    {
        var end = DateTime.UtcNow + TimeSpan.FromMilliseconds(durationMs);

        while (DateTime.UtcNow < end)
        {
            Timer.Slice();
            Thread.Sleep(10);
        }
    }

    [Test]
    public void DelayCall_InvokesCallback_AfterDelay()
    {
        var fired = false;
        Timer.DelayCall(TimeSpan.FromMilliseconds(10), () => fired = true);

        ProcessTimers(200);

        Assert.IsTrue(fired, "Timer callback not invoked");
    }

    [Test]
    public void DelayCall_Repeats_ForSpecifiedCount()
    {
        var count = 0;
        Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(10), 3, () =>
        {
            Interlocked.Increment(ref count);
        });

        ProcessTimers(200);

        Assert.AreEqual(3, count);
    }
}
