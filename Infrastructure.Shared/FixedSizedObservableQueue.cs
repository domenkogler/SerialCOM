namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public class FixedSizedObservableQueue<T> : ObservableQueue<T>
    {
        //private readonly object syncObject = new object();

        public int Size { get; }

        public FixedSizedObservableQueue(int size)
        {
            Size = size;
        }

        public override void Enqueue(T obj)
        {
            base.Enqueue(obj);
            //lock (syncObject)
            //{
            while (Count > Size)
            {
                //T outObj;
                //base.TryDequeue(out outObj);
                Dequeue();
            }
            //}
        }
    }
}