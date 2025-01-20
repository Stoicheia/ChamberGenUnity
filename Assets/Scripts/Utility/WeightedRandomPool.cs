using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public class WeightedRandomPool<T>
    {
        private List<WeightedRandomPoolEntry<T>> _pool;
        private List<T> _allItems;
        private Random _random;

        public WeightedRandomPool(List<WeightedRandomPoolEntry<T>> pool)
        {
            _pool = pool;
            _allItems = _pool.Select(x => x.Item).ToList();
            _random = new Random();
        }

        public WeightedRandomPool(List<T> equalWeightItems)
        {
            _pool = equalWeightItems.Select(x => new WeightedRandomPoolEntry<T>(x, 1)).ToList();
            _allItems = _pool.Select(x => x.Item).ToList();
            _random = new Random();
        }
        
        public WeightedRandomPool(Dictionary<T, float> itemsByWeight)
        {
            _pool = itemsByWeight.Select(x => new WeightedRandomPoolEntry<T>(x.Key, x.Value)).ToList();
            _allItems = _pool.Select(x => x.Item).ToList();
            _random = new Random();
        }
        
        public T GetRandom(Random rng = null)
        {
            if (rng != null) _random = rng;
            return GetRandomInSubset(_random, _allItems);
        }

        public T GetRandomInSubset(Random rng, List<T> includes)
        {
            List<WeightedRandomPoolEntry<T>> subset = _pool.Where(x => includes.Contains(x.Item)).ToList();
            
            float cumulativeWeight = subset.Sum(x => x.Weight);
            float rand = (float)rng.NextDouble() * cumulativeWeight;
            float sum = 0;
            for (int i = 0; i < subset.Count; i++)
            {
                sum += subset[i].Weight;
                if (sum > rand)
                {
                    return subset[i].Item;
                }
            }
            return subset.Last().Item;
        }

        public List<T> GetAll()
        {
            return _pool.Select(x => x.Item).ToList();
        }
        
        public struct WeightedRandomPoolEntry<T>
        {
            public T Item;
            public float Weight;

            public WeightedRandomPoolEntry(T item, float weight)
            {
                this.Item = item;
                this.Weight = weight;
            }
        }
    }
}