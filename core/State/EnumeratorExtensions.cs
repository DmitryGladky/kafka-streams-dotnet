﻿using Streamiz.Kafka.Net.State.Enumerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streamiz.Kafka.Net.State
{
    public static class EnumeratorExtensions
    {
        public static List<KeyValuePair<K, V>> ToList<K, V>(this IKeyValueEnumerator<K, V> enumerator)
        {
            List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();

            while (enumerator.MoveNext())
            {
                if(enumerator.Current.HasValue)
                    list.Add(enumerator.Current.Value);
            }

            enumerator.Dispose();
            return list;
        }
    }
}