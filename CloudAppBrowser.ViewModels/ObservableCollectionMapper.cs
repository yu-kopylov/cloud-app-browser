using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CloudAppBrowser.ViewModels
{
    public class ObservableCollectionMapper<TEntity, TViewModel>
    {
        private readonly ConstructModel constructModel;
        private readonly GetEntity getEntity;
        private readonly UpdateModel updateModel;
        private readonly Comparison<TViewModel> comparison;

        public ObservableCollectionMapper(ConstructModel constructModel, GetEntity getEntity, UpdateModel updateModel, Comparison<TViewModel> comparison)
        {
            this.constructModel = constructModel;
            this.getEntity = getEntity;
            this.updateModel = updateModel;
            this.comparison = comparison;
        }

        public delegate TViewModel ConstructModel(TEntity entity);

        public delegate TEntity GetEntity(TViewModel model);

        public delegate void UpdateModel(TEntity entity, TViewModel model);

        public void UpdateCollection(IEnumerable<TEntity> entities, ObservableCollection<TViewModel> target)
        {
            Dictionary<TEntity, TViewModel> existingModels = target.ToDictionary(model => getEntity(model));

            List<TViewModel> presentModels = new List<TViewModel>();

            foreach (TEntity entity in entities)
            {
                TViewModel model;
                if (existingModels.TryGetValue(entity, out model))
                {
                    updateModel(entity, model);
                }
                else
                {
                    model = constructModel(entity);
                }
                presentModels.Add(model);
            }

            if (comparison != null)
            {
                presentModels.Sort(comparison);
            }

            List<TViewModel> removedModels = UpdateTargetContent(presentModels, target);
            ReorderTarget(presentModels, target);

            foreach (TViewModel removedModel in removedModels)
            {
                IDisposable disposableModel = removedModel as IDisposable;
                disposableModel?.Dispose();
            }
        }

        private List<TViewModel> UpdateTargetContent(List<TViewModel> presentModels, ObservableCollection<TViewModel> target)
        {
            Dictionary<TViewModel, int> presentModelIndices = new Dictionary<TViewModel, int>();
            for (int i = 0; i < presentModels.Count; i++)
            {
                presentModelIndices.Add(presentModels[i], i);
            }

            HashSet<TViewModel> existingModels = new HashSet<TViewModel>();
            foreach (TViewModel model in target)
            {
                existingModels.Add(model);
            }

            List<TViewModel> longestOrderedChain = FindLongestOrderedChain(presentModels, presentModelIndices, target);

            List<TViewModel> removedModels = new List<TViewModel>();
            int insertPosition = 0;
            int sourcePosition = 0;
            Queue<int> replacePositions = new Queue<int>();
            foreach (TViewModel fixedModel in longestOrderedChain)
            {
                replacePositions.Clear();
                while (!target[insertPosition].Equals(fixedModel))
                {
                    if (!presentModelIndices.ContainsKey(target[insertPosition]))
                    {
                        replacePositions.Enqueue(insertPosition);
                    }
                    insertPosition++;
                }
                while (!presentModels[sourcePosition].Equals(fixedModel))
                {
                    if (existingModels.Contains(presentModels[sourcePosition]))
                    {
                        sourcePosition++;
                    }
                    else if (replacePositions.Count > 0)
                    {
                        int replacePosition = replacePositions.Dequeue();
                        removedModels.Add(target[replacePosition]);
                        target[replacePosition] = presentModels[sourcePosition];
                        sourcePosition++;
                    }
                    else
                    {
                        target.Insert(insertPosition, presentModels[sourcePosition]);
                        insertPosition++;
                        sourcePosition++;
                    }
                }
                sourcePosition++;
                insertPosition++;
            }
            replacePositions.Clear();
            while (insertPosition < target.Count)
            {
                if (!presentModelIndices.ContainsKey(target[insertPosition]))
                {
                    replacePositions.Enqueue(insertPosition);
                }
                insertPosition++;
            }
            while (sourcePosition < presentModels.Count)
            {
                if (existingModels.Contains(presentModels[sourcePosition]))
                {
                    sourcePosition++;
                }
                else if (replacePositions.Count > 0)
                {
                    int replacePosition = replacePositions.Dequeue();
                    removedModels.Add(target[replacePosition]);
                    target[replacePosition] = presentModels[sourcePosition];
                    sourcePosition++;
                }
                else
                {
                    target.Insert(insertPosition, presentModels[sourcePosition]);
                    insertPosition++;
                    sourcePosition++;
                }
            }
            int removePosition = target.Count - 1;
            while (removePosition >= 0)
            {
                if (!presentModelIndices.ContainsKey(target[removePosition]))
                {
                    removedModels.Add(target[removePosition]);
                    target.RemoveAt(removePosition);
                }
                removePosition--;
            }
            return removedModels;
        }

        private void ReorderTarget(List<TViewModel> presentModels, ObservableCollection<TViewModel> target)
        {
            Dictionary<TViewModel, int> presentModelIndices = new Dictionary<TViewModel, int>();
            for (int i = 0; i < presentModels.Count; i++)
            {
                presentModelIndices.Add(presentModels[i], i);
            }

            List<TViewModel> longestOrderedChain = FindLongestOrderedChain(presentModels, presentModelIndices, target);
            List<int> longestOrderedIndexChain = longestOrderedChain.Select(m => presentModelIndices[m]).ToList();

            for (int index = 0; index < presentModels.Count; index++)
            {
                TViewModel model = target[index];
                int presentModelIndex = presentModelIndices[model];
                var insertPosition = FindInsertPosition(longestOrderedIndexChain, presentModelIndex);
                if (insertPosition < longestOrderedIndexChain.Count && longestOrderedIndexChain[insertPosition] == presentModelIndex)
                {
                    continue;
                }
                int newIndex = insertPosition < longestOrderedIndexChain.Count ? target.IndexOf(presentModels[longestOrderedIndexChain[insertPosition]]) : target.Count;
                if (newIndex > index)
                {
                    target.Move(index, newIndex - 1);
                    index--;
                }
                else
                {
                    target.Move(index, newIndex);
                }
                longestOrderedIndexChain.Insert(insertPosition, presentModelIndices[model]);
            }
        }

        private static List<TViewModel> FindLongestOrderedChain(
            List<TViewModel> presentModels,
            Dictionary<TViewModel, int> presentModelIndices,
            ObservableCollection<TViewModel> target)
        {
            Dictionary<int, int> parents = new Dictionary<int, int>();
            List<int> lastElementByLength = new List<int>();
            foreach (TViewModel model in target)
            {
                int modelIndex;
                if (!presentModelIndices.TryGetValue(model, out modelIndex))
                {
                    continue;
                }

                int insertPosition = FindInsertPosition(lastElementByLength, modelIndex);

                if (insertPosition >= lastElementByLength.Count)
                {
                    lastElementByLength.Add(modelIndex);
                }
                else
                {
                    lastElementByLength[insertPosition] = modelIndex;
                }
                if (insertPosition > 0)
                {
                    parents.Add(modelIndex, lastElementByLength[insertPosition - 1]);
                }
            }
            List<TViewModel> longestChain = new List<TViewModel>();
            if (lastElementByLength.Count > 0)
            {
                int modelIndex = lastElementByLength[lastElementByLength.Count - 1];
                longestChain.Add(presentModels[modelIndex]);
                while (parents.TryGetValue(modelIndex, out modelIndex))
                {
                    longestChain.Add(presentModels[modelIndex]);
                }
            }
            longestChain.Reverse();
            return longestChain;
        }

        /// <summary>
        /// Finds the position within the given sorted array where the given element should be inserted using binary search.
        /// If the element is already in the array, returns its position.
        /// </summary>
        /// <param name="values">The sorted array.</param>
        /// <param name="value">The value to look for.</param>
        private static int FindInsertPosition(List<int> values, int value)
        {
            int low = 0;
            int hi = values.Count - 1;
            while (low <= hi)
            {
                int mid = (hi + low) / 2;
                if (values[mid] < value)
                {
                    low = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return low;
        }
    }
}