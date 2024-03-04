using PalettaPolizeiPro.Services;

namespace PalettaPolizeiPro
{
    public class LongRunningJobHandler
    {
        private List<IUpdatable> _updatables { get; set; }
        private int _threadSleep = 0; 
        Thread? _mainThread;
        public LongRunningJobHandler(List<IUpdatable> updatables,int threadsleep)
        {
            _updatables = updatables; 
        }
     
        public void Start()
        {
            _mainThread = new Thread(Run);
            _mainThread.Start();
        }
        private void Run()
        {
            while (ProgramRunning)
            { 
                foreach (var item in _updatables)
                {
                    item.Update();
                }
                if (_threadSleep > 0)
                {
                    Thread.Sleep(_threadSleep);
                }
            }
        }
    }
}   