using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CloudAppBrowser.ViewModels
{
    public class ObservableCollectionWatcher<T> where T : INotifyPropertyChanged
    {
        private readonly Action onItemChanged;
        private ObservableCollection<T> collection;

        public ObservableCollectionWatcher(Action onItemChanged)
        {
            this.onItemChanged = onItemChanged;
        }

        public void SetCollection(ObservableCollection<T> collection)
        {
            if (this.collection != null)
            {
                DetachItemHandlers(this.collection);
                this.collection.CollectionChanged -= OnCollectionContentChanged;
            }

            this.collection = collection;

            if (this.collection != null)
            {
                this.collection.CollectionChanged += OnCollectionContentChanged;
                AttachItemHandlers(this.collection);
            }
        }

        private void OnCollectionContentChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            DetachItemHandlers(args.OldItems);
            AttachItemHandlers(args.NewItems);
        }

        private void AttachItemHandlers(IList items)
        {
            if (items == null)
            {
                return;
            }

            foreach (INotifyPropertyChanged item in items)
            {
                item.PropertyChanged += OnItemChanged;
            }
        }

        private void DetachItemHandlers(IList items)
        {
            if (items == null)
            {
                return;
            }

            foreach (INotifyPropertyChanged item in items)
            {
                item.PropertyChanged -= OnItemChanged;
            }
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs args)
        {
            onItemChanged();
        }
    }
}