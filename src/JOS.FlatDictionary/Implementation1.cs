﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JOS.FlatDictionary
{
    public class Implementation1 : IFlatDictionaryProvider
    {
        public Dictionary<string, string> Execute(object @object, string prefix = "")
        {
            var dictionary = new Dictionary<string, string>();
            Flatten(dictionary, @object, prefix);
            return dictionary;
        }

        private static void Flatten(
            IDictionary<string, string> dictionary,
            object source,
            string name)
        {
            var properties = source.GetType().GetProperties().Where(x => x.CanRead);
            foreach (var property in properties)
            {
                var key = string.IsNullOrWhiteSpace(name) ? property.Name : $"{name}.{property.Name}";
                var value = property.GetValue(source, null);

                if (value == null)
                {
                    dictionary[key] = null;
                    continue;
                }

                if (property.PropertyType.IsValueTypeOrString())
                {
                    dictionary[key] = value.ToStringValueType();
                }
                else
                {
                    if (value is IEnumerable enumerable)
                    {
                        var counter = 0;
                        foreach (var item in enumerable)
                        {
                            var itemKey = $"{key}[{counter++}]";
                            if (item.GetType().IsValueTypeOrString())
                            {
                                dictionary.Add(itemKey, item.ToStringValueType());
                            }
                            else
                            {
                                Flatten(dictionary, item, itemKey);
                            }
                        }
                    }
                    else
                    {
                        Flatten(dictionary, value, key);
                    }
                }
            }
        }
    }
}