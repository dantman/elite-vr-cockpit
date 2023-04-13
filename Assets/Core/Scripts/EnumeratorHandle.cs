using System;

namespace EVRC.Core
{
    /**
     * A simple helper to allow coroutines to be run with a handle that can tell them to stop gracefully
     */
    public class CoroutineHandle
    {
        public bool Running { get; private set; } = true;

        public void Stop()
        {
            if (!Running)
            {
                throw new Exception("Coroutine was already stopped");
            }

            Running = false;
        }
    }
}
