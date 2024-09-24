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
            _threadSleep = threadsleep;
        }
     
        public void Start()
        {
            _mainThread = new Thread(async()=> { await Run(); });
            _mainThread.Start();
        }
        private async Task Run()
        {
            while (ProgramRunning)
            { 
                foreach (var item in _updatables)
                {
                    await item.Update();
                }
                if (_threadSleep > 0)
                {
                    Thread.Sleep(_threadSleep);
                }
            }
        }
    }
}   