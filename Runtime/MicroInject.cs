using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    public static class MicroInject
    {
        internal static Dictionary<string, Component> GlobalBuckets = new  Dictionary<string, Component>();

        public static void RegisterGlobalBucket(string bucketName, Component container) => GlobalBuckets.Add(bucketName, container);

        public static T GetGlobalBucket<T>(string bucketName) where T : Component => (T)GlobalBuckets[bucketName];

        internal static void RebuildDependencyList()
        {
            var filteredDependencies = GlobalBuckets.Where(kvp => kvp.Value == null).ToArray();
            GlobalBuckets.Clear();
            foreach (var kvp in filteredDependencies)
            {
                GlobalBuckets.Add(kvp.Key, kvp.Value);
            }
        }

#if UNITY_EDITOR
      
        #region DebugGet

        public static Dictionary<string, Component> GetGlobalBuckets() => GlobalBuckets;

        #endregion

#endif
    }
}