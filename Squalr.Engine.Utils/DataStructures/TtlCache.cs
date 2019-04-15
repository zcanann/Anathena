namespace Squalr.Engine.Common.DataStructures
{
    using System;
    using System.Collections.Concurrent;

    public class TtlCache<V>
    {
        private ConcurrentDictionary<V, DateTime> cache;

        public TtlCache()
        {
            this.cache = new ConcurrentDictionary<V, DateTime>();
            this.DefaultTimeToLive = TimeSpan.MaxValue;
        }

        public TtlCache(TimeSpan defaultTimeToLive) : this()
        {
            this.DefaultTimeToLive = defaultTimeToLive;
        }

        public TtlCache(TimeSpan defaultTimeToLive, TimeSpan randomTimeToLiveOffset) : this(defaultTimeToLive)
        {
            this.RandomTimeToLiveOffset = randomTimeToLiveOffset;
        }

        protected TimeSpan DefaultTimeToLive { get; set; }

        protected TimeSpan RandomTimeToLiveOffset { get; set; }

        protected static Random Random = new Random();

        public void Invalidate()
        {
            this.cache.Clear();
        }

        public void Add(V value)
        {
            if (this.RandomTimeToLiveOffset != null)
            {
                Int32 maximumOffset = (Int32)this.RandomTimeToLiveOffset.TotalMilliseconds;
                TimeSpan offset = TimeSpan.FromMilliseconds(TtlCache<V>.Random.Next(-maximumOffset, maximumOffset));
                TimeSpan timeToLive = this.DefaultTimeToLive + offset;

                this.Add(value, timeToLive);
            }
            else
            {
                this.Add(value, this.DefaultTimeToLive);
            }
        }

        public void Add(V value, TimeSpan timeToLive)
        {
            DateTime expireTime = timeToLive == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + timeToLive;

            this.cache.AddOrUpdate(value, expireTime, (key, ttl) => { return ttl; });
        }

        public Boolean Contains(V value)
        {
            if (this.cache.ContainsKey(value))
            {
                DateTime result;

                if (this.cache.TryGetValue(value, out result))
                {
                    if (DateTime.Now <= result || result == DateTime.MaxValue)
                    {
                        // Cache contains valid unexpired entry
                        return true;
                    }

                    // Cache expired
                    this.cache.TryRemove(value, out result);
                }
            }

            return false;
        }
    }

    public class TtlCache<K, V> : TtlCache<K> where V : class
    {
        private ConcurrentDictionary<K, Tuple<V, DateTime>> cache;

        public TtlCache() : base()
        {
            this.cache = new ConcurrentDictionary<K, Tuple<V, DateTime>>();
        }

        public TtlCache(TimeSpan defaultTimeToLive) : base(defaultTimeToLive)
        {
            this.cache = new ConcurrentDictionary<K, Tuple<V, DateTime>>();
        }

        public TtlCache(TimeSpan defaultTimeToLive, TimeSpan randomTimeToLiveOffset) : base(defaultTimeToLive, randomTimeToLiveOffset)
        {
            this.cache = new ConcurrentDictionary<K, Tuple<V, DateTime>>();
        }

        public new Boolean Contains(K key)
        {
            if (this.cache.ContainsKey(key))
            {
                Tuple<V, DateTime> result;

                if (this.cache.TryGetValue(key, out result))
                {
                    if (DateTime.Now <= result.Item2 || result.Item2 == DateTime.MaxValue)
                    {
                        // Cache contains valid unexpired entry
                        return true;
                    }

                    // Cache expired
                    this.cache.TryRemove(key, out result);
                }
            }

            return false;
        }

        public void Add(K key, V value)
        {
            if (this.RandomTimeToLiveOffset != null)
            {
                Int32 maximumOffset = (Int32)this.RandomTimeToLiveOffset.TotalMilliseconds;
                TimeSpan offset = TimeSpan.FromMilliseconds(TtlCache<K, V>.Random.Next(-maximumOffset, maximumOffset));
                TimeSpan timeToLive = this.DefaultTimeToLive + offset;

                this.Add(key, value, timeToLive);
            }
            else
            {
                this.Add(key, value, this.DefaultTimeToLive);
            }
        }

        public void Add(K key, V value, TimeSpan timeToLive)
        {
            DateTime expireTime = timeToLive == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + timeToLive;
            Tuple<V, DateTime> newValue = new Tuple<V, DateTime>(value, expireTime);

            this.cache.AddOrUpdate(key, newValue, (temp, ttl) => { return ttl; });
        }

        public Boolean TryGetValue(K key, out V value)
        {
            value = null;

            if (this.cache.TryGetValue(key, out Tuple<V, DateTime> result))
            {
                if (DateTime.Now <= result.Item2 || result.Item2 == DateTime.MaxValue)
                {
                    // Cache contains valid unexpired entry
                    value = result.Item1;
                    return true;
                }

                // Cache expired
                this.cache.TryRemove(key, out result);
            }

            return false;
        }
    }
    //// End class
}
//// End namespace