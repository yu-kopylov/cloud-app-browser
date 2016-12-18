using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CloudAppBrowser.ViewModels.Tests
{
    [TestFixture]
    public class ObservableCollectionMapperTests
    {
        private readonly Random random = new Random();

        [Test]
        public void Test()
        {
            TestCase(
                new string[] {},
                new string[] {"1", "2", "3", "4", "5"},
                new string[]
                {
                    "Add: {null} -> {0: 1}",
                    "Add: {null} -> {1: 2}",
                    "Add: {null} -> {2: 3}",
                    "Add: {null} -> {3: 4}",
                    "Add: {null} -> {4: 5}"
                },
                new string[] {}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {},
                new string[] {}
                );

            TestCase(
                new string[] {"2", "4"},
                new string[] {"1", "2", "3", "4", "5"},
                new string[]
                {
                    "Add: {null} -> {0: 1}",
                    "Add: {null} -> {2: 3}",
                    "Add: {null} -> {4: 5}"
                },
                new string[] {}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"2", "4"},
                new string[]
                {
                    "Remove: {4: 5} -> {null}",
                    "Remove: {2: 3} -> {null}",
                    "Remove: {0: 1} -> {null}"
                },
                new string[] {"1", "3", "5"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"-1", "-2", "3", "-4", "-5"},
                new string[]
                {
                    "Replace: {0: 1} -> {0: -1}",
                    "Replace: {1: 2} -> {1: -2}",
                    "Replace: {3: 4} -> {3: -4}",
                    "Replace: {4: 5} -> {4: -5}",
                },
                new string[] {"1", "2", "4", "5"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"2", "3", "4", "5", "1"},
                new string[]
                {
                    "Move: {0: 1} -> {4: 1}"
                },
                new string[] {}
                );

            TestCase(
                new string[] {"2", "3", "4", "5", "1"},
                new string[] {"1", "2", "3", "4", "5"},
                new string[]
                {
                    "Move: {4: 1} -> {0: 1}"
                },
                new string[] {}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"-1", "5", "2", "3", "4"},
                new string[]
                {
                    "Replace: {0: 1} -> {0: -1}",
                    "Move: {4: 5} -> {1: 5}",
                },
                new string[] {"1"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"5", "1", "2", "3", "-4"},
                new string[]
                {
                    "Replace: {3: 4} -> {3: -4}",
                    "Move: {4: 5} -> {0: 5}"
                },
                new string[] {"4"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"5", "1", "2", "3"},
                new string[]
                {
                    "Remove: {3: 4} -> {null}",
                    "Move: {3: 5} -> {0: 5}",
                },
                new string[] {"4"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5"},
                new string[] {"3", "4", "5", "1"},
                new string[]
                {
                    "Remove: {1: 2} -> {null}",
                    "Move: {0: 1} -> {3: 1}"
                },
                new string[] {"2"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5", "6", "7"},
                new string[] {"7", "-2", "4", "3", "-5", "6", "1"},
                null,
                new string[] {"2", "5"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5", "6", "7"},
                new string[] {"7", "-2", "4", "3", "-5", "6", "0", "1"},
                null,
                new string[] {"2", "5"}
                );

            TestCase(
                new string[] {"1", "2", "3", "4", "5", "6", "7"},
                new string[] {"7", "4", "3", "6", "1"},
                null,
                new string[] {"2", "5"}
                );
        }

        [Test]
        [Explicit]
        public void TestPerformance()
        {
            const int iterationCount = 1000;
            const int availableValuesCount = 2000;
            const int sourceListSize = 1000;
            const int destListSize = 1000;

            Stopwatch stopwatch = new Stopwatch();
            long operationCount = 0;

            for (int it = 0; it < iterationCount; it++)
            {
                List<string> sourceList = GenerateSet(sourceListSize, availableValuesCount, 0.5, 3);
                List<string> destList = GenerateSet(destListSize, availableValuesCount, 0.65, 1);

                ObservableCollection<string> collection = new ObservableCollection<string>();

                ObservableCollectionMapper<string, string> mapper = new ObservableCollectionMapper<string, string>
                    (
                    entity => entity,
                    model => model,
                    (entity, model) => { },
                    null
                    );

                mapper.UpdateCollection(sourceList, collection);

                collection.CollectionChanged += (sender, args) => operationCount++;

                stopwatch.Start();
                mapper.UpdateCollection(destList, collection);
                stopwatch.Stop();

                Assert.That(collection, Is.EqualTo(destList));
            }

            Console.WriteLine("Update Time:     {0} ms ({1} ms per iteration)", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds/iterationCount);
            Console.WriteLine("Operation Count: {0} ({1} per iteration)", operationCount, operationCount/iterationCount);
        }

        private List<string> GenerateSet(int listSize, int availableValuesCount, double pFirst, int numFirst)
        {
            List<string> values = new List<string>();
            for (int i = 0; i < availableValuesCount; i++)
            {
                values.Add(i.ToString());
            }
            List<string> res = new List<string>();
            for (int i = 0; i < listSize; i++)
            {
                int range = values.Count;
                if (random.NextDouble() < pFirst)
                {
                    range = Math.Min(range, numFirst);
                }
                int idx = random.Next(range);
                res.Add(values[idx]);
                values.RemoveAt(idx);
            }
            return res;
        }

        private void TestCase(string[] sourceContent, string[] destinationContent, string[] expectedOperations, string[] expectedDisposals)
        {
            List<string> disposals = new List<string>();

            ObservableCollectionMapper<string, SimpleViewModel> mapper = new ObservableCollectionMapper<string, SimpleViewModel>(
                entity => new SimpleViewModel(disposals, entity, "D-" + entity),
                model => model.Entity,
                (entity, model) => { },
                null);

            ObservableCollection<SimpleViewModel> target = new ObservableCollection<SimpleViewModel>();

            mapper.UpdateCollection(sourceContent, target);
            Assert.That(disposals, Is.Empty);

            List<string> operations = new List<string>();
            target.CollectionChanged += (sender, args) => operations.Add(FormatChangeEvent(args));

            mapper.UpdateCollection(destinationContent, target);
            Assert.That(target.Select(m => m.Entity).ToList(), Is.EqualTo(destinationContent));
            if (expectedOperations != null)
            {
                Assert.That(operations, Is.EqualTo(expectedOperations), "operations does not match expectations");
            }
            Assert.That(disposals, Is.EquivalentTo(expectedDisposals), "disposals does not match expectations");
        }

        private string FormatChangeEvent(NotifyCollectionChangedEventArgs args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(args.Action);
            sb.Append(": {");
            sb.Append(FormatItems(args.OldStartingIndex, args.OldItems));
            sb.Append("} -> {");
            sb.Append(FormatItems(args.NewStartingIndex, args.NewItems));
            sb.Append("}");
            return sb.ToString();
        }

        private string FormatItems(int startingIndex, IList items)
        {
            if (items == null)
            {
                return "null";
            }

            List<string> itemNames = new List<string>();
            foreach (object item in items)
            {
                itemNames.Add(item.ToString());
            }
            return startingIndex + ": " + string.Join(", ", itemNames);
        }

        private class SimpleViewModel : IDisposable
        {
            private readonly List<string> disposals;

            public string Entity { get; }
            public string Data { get; }

            public SimpleViewModel(List<string> disposals, string entity, string data)
            {
                this.disposals = disposals;
                Entity = entity;
                Data = data;
            }

            public void Dispose()
            {
                disposals.Add(Entity);
            }

            public override string ToString()
            {
                return Entity;
            }
        }
    }
}