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

        Assert.That(count, Is.EqualTo(3));
    }

    private sealed class DummyTimer : Timer
    {
        public int Count;

        public DummyTimer(TimeSpan delay, TimeSpan interval)
            : base(delay, interval)
        {
        }

        protected override void OnTick()
        {
            Interlocked.Increment(ref Count);
        }
    }

    [Test]
    public void PriorityChange_ToSlower_DelaysExecution()
    {
        var count = 0;
        Timer timer = null!;
        timer = Timer.DelayCall(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), () =>
        {
            Interlocked.Increment(ref count);
            if (count == 1)
            {
                timer.Priority = TimerPriority.OneSecond;
            }
        });

        ProcessTimers(500);
        Assert.That(count, Is.EqualTo(1), "Timer should not tick again before priority delay");

        ProcessTimers(1000);
        Assert.That(count, Is.EqualTo(2), "Timer did not tick after priority delay elapsed");
    }

    [Test]
    public void PriorityChange_ToFaster_SpeedsExecution()
    {
        var timer = new DummyTimer(TimeSpan.Zero, TimeSpan.FromMilliseconds(10))
        {
            Priority = TimerPriority.OneSecond
        };

        timer.Start();

        ProcessTimers(1200);
        Assert.That(timer.Count, Is.EqualTo(1));

        timer.Priority = TimerPriority.TenMS;

        ProcessTimers(100);
        Assert.GreaterOrEqual(timer.Count, 2);
    }
}
