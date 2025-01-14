﻿using Streamiz.Kafka.Net.Crosscutting;
using Streamiz.Kafka.Net.State.Enumerator;
using Streamiz.Kafka.Net.State.Internal;
using System.Collections.Generic;
using Streamiz.Kafka.Net.SerDes;

namespace Streamiz.Kafka.Net.State.Logging
{
    internal class ChangeLoggingTimestampedKeyValueBytesStore :
                WrappedStateStore<IKeyValueStore<Bytes, byte[]>>,
                IKeyValueStore<Bytes, byte[]>
    {
        public ChangeLoggingTimestampedKeyValueBytesStore(IKeyValueStore<Bytes, byte[]> wrapped)
            : base(wrapped)
        {
        }

        private void Publish(Bytes key, byte[] value)
        {
            if(value == null)
                context.Log(Name, key, null, context.RecordContext.Timestamp);
            else
            {
                (long ts, byte[] data) = ValueAndTimestampSerDes.Extract(value);
                context.Log(Name, key, data, ts);
            }
        }

        public IEnumerable<KeyValuePair<Bytes, byte[]>> All()
            => wrapped.All();

        public long ApproximateNumEntries()
            => wrapped.ApproximateNumEntries();

        public byte[] Delete(Bytes key)
        {
            byte[] oldValue = wrapped.Delete(key);
            Publish(key, null);
            return oldValue;
        }

        public byte[] Get(Bytes key)
            => wrapped.Get(key);

        public void Put(Bytes key, byte[] value)
        {
            wrapped.Put(key, value);
            Publish(key, value);
        }

        public void PutAll(IEnumerable<KeyValuePair<Bytes, byte[]>> entries)
        {
            wrapped.PutAll(entries);
            foreach (var e in entries)
            {
                Publish(e.Key, e.Value);
            }
        }

        public byte[] PutIfAbsent(Bytes key, byte[] value)
        {
            var previous = wrapped.PutIfAbsent(key, value);
            if (previous == null)
            {
                Publish(key, value);
            }

            return previous;
        }

        public IKeyValueEnumerator<Bytes, byte[]> Range(Bytes from, Bytes to)
            => wrapped.Range(from, to);

        public IEnumerable<KeyValuePair<Bytes, byte[]>> ReverseAll()
            => wrapped.ReverseAll();

        public IKeyValueEnumerator<Bytes, byte[]> ReverseRange(Bytes from, Bytes to)
            => wrapped.ReverseRange(from, to);
    }
}