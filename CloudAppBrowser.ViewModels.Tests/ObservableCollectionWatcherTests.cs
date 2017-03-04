using System.Collections.ObjectModel;
using System.ComponentModel;
using NUnit.Framework;

namespace CloudAppBrowser.ViewModels.Tests
{
    [TestFixture]
    public class ObservableCollectionWatcherTests
    {
        [Test]
        public void TestSmoke()
        {
            int changeCount = 0;
            ObservableCollectionWatcher<Sample> watcher = new ObservableCollectionWatcher<Sample>(() => changeCount++);

            ObservableCollection<Sample> collection = new ObservableCollection<Sample>();

            watcher.SetCollection(collection);
            Assert.That(changeCount, Is.EqualTo(0));

            Sample sample1 = new Sample();
            collection.Add(sample1);
            Assert.That(changeCount, Is.EqualTo(0));

            Sample sample2 = new Sample();
            collection.Add(sample2);
            Assert.That(changeCount, Is.EqualTo(0));

            sample1.FirePropertyChanged();
            Assert.That(changeCount, Is.EqualTo(1));

            sample2.FirePropertyChanged();
            Assert.That(changeCount, Is.EqualTo(2));

            collection.Remove(sample1);
            sample1.FirePropertyChanged();
            Assert.That(changeCount, Is.EqualTo(2));

            Sample sample3 = new Sample();
            collection[0] = sample3;
            Assert.That(changeCount, Is.EqualTo(2));

            sample1.FirePropertyChanged();
            sample2.FirePropertyChanged();
            Assert.That(changeCount, Is.EqualTo(2));

            sample3.FirePropertyChanged();
            Assert.That(changeCount, Is.EqualTo(3));

            watcher.SetCollection(null);
            Assert.That(changeCount, Is.EqualTo(3));
        }

        private class Sample : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public void FirePropertyChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("test"));
            }
        }
    }
}