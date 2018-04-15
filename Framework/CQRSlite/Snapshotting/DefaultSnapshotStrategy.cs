using System;
using System.Reflection;
using CQRSlite.Domain;

namespace CQRSlite.Snapshotting
{
    /// <summary>
    /// Default implementaion of snapshot strategy interface/
    /// Snapshots aggregates of type SnapshotAggregateRoot every 100th event.
    /// </summary>
    public class DefaultSnapshotStrategy : ISnapshotStrategy
    {
        private const int snapshotInterval = 100;

        public bool IsSnapshotable(Type aggregateType)
        {
            var aggregateBaseType = aggregateType.GetTypeInfo().BaseType;
            if (aggregateBaseType == null)
                return false;

            if (aggregateBaseType.GetTypeInfo().IsGenericType &&
                aggregateBaseType.GetGenericTypeDefinition() == typeof(SnapshotAggregateRoot<>))
                return true;

            return IsSnapshotable(aggregateBaseType);
        }

        public bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            if (!IsSnapshotable(aggregate.GetType()))
                return false;

            var i = aggregate.Version;
            for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
                if (++i % snapshotInterval == 0 && i != 0)
                    return true;
            return false;
        }
    }
}