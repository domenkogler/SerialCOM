using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kogler.SerialCOM
{
    public class ObservableQueue<T> : Queue<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, object items, int? index = null)
        {
            if (index.HasValue)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items, index.Value));
            }
            else
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items));
            }
            OnPropertyChanged(nameof(Count));
        }

        protected virtual void OnPropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public new virtual void Enqueue(T item)
        {
            base.Enqueue(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new[] { item }, 0);
        }

        public new virtual T Dequeue()
        {
            var r = base.Dequeue();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, new[] { r }, Count - 1);
            return r;
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }
    }

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