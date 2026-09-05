public class SensorStateService
{   private readonly object _lock = new object();

    private string _currentMessage = "Default: Hello you are subscribed";
    private int _gear = 0;
    public string CurrentMessage {get
        {
            lock (_lock)
            {
                return _currentMessage;
            }   
        } 
        set
        {
            lock (_lock)
            {
                _currentMessage = value;
            }
        }
    }

    public int Gear
    {
        get
        {
            lock (_lock)
            {
                return _gear;
            }
        }
        set
        {
            lock (_lock)
            {
                _gear = value;
            }
        }
    }
}