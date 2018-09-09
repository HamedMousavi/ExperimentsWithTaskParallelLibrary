using System.Threading;

namespace MsTpl
{
    public class Semaphores
    {

        private readonly Semaphore _sem = new Semaphore(0, 3);
        private readonly AutoResetEvent _cancelEvent = new AutoResetEvent(false);


        public void RunTests()
        {
            // Instead of the blocking collection, this can be used. Benefit: instead of throwing exception to cancel we can use an Autoreset event
            Enqueue(1);
            Enqueue(2);
            Enqueue(3);
            //Enqueue(3); exception too many items
            //Enqueue(3);

            var first = Dequeue<int>();

            Enqueue(4);
        }



        private void Enqueue<T>(T t)
        {
            // DO THIS IN A LOCK OR MONITOR
            try
            {
                //_queue.Enqueu(t);
                // Will be called upon enqueue
                _sem.Release();

            }
            catch (SemaphoreFullException)
            {
                /* throw custom exception */
            }

        }

        private T Dequeue<T>()
        {
            var index = WaitHandle.WaitAny(new WaitHandle[] {_sem, _cancelEvent});
            if (index == 1) return default(T);

            //return _queue.Dequeue();
            return default(T);
        }
    }
}
