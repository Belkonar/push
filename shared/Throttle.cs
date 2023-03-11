namespace shared;

public class Throttle
{
    private readonly Func<Task> _func;
    private readonly Object _locker = new object();
    private DateTime _time = DateTime.Now;

    public int Interval { get; set; } = 3;

    public Throttle(Func<Task> func)
    {
        _func = func;
    }

    public async Task Call()
    {
        DateTime start;

        lock (_locker)
        {
            start = _time;
        }

        var end = DateTime.Now;

        var seconds = (end - start).TotalSeconds;

        if (seconds >= Interval)
        {
            lock (_locker)
            {
                _time = end;
            }
            
            await _func();    
        }
    }
}