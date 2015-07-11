using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kogler.SerialCOM
{
    public class ObservableStack<T> : Stack<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public ObservableStack(int limit)
        {
            Limit = limit;
        }

        public int Limit { get; }
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

        public new virtual void Clear()
        {
            base.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        public new virtual T Pop()
        {
            var result = base.Pop();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, new [] { result }, Count);
            return result;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new [] { item }, Count - 1);
        }
    }
}