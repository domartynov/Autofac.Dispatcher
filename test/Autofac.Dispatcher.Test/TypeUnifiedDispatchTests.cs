using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Autofac.Dispatcher.Test
{
    public class TypeUnifiedDispatchTests
    {
        [Fact]
        public void TestGenericHandlerDispatchedASpecializedCommand()
        {
            
        }

        public abstract class Entity<TKey>
        {
            public TKey Id { get; set; }
        }

        public class FooEntity : Entity<long>
        {
            
        }

        public class AllEntitiesLoaded<TEntity, TKey> : IEvent where TEntity : Entity<TKey>
        {
            public IEnumerable<TEntity> Entities { get; private set; }

            public AllEntitiesLoaded(IEnumerable<TEntity> entities)
            {
                Entities = entities;
            }
        }

        public class MemoryStore<TEntity, TKey> where TEntity : Entity<TKey>
        {
            private readonly TaskCompletionSource<ConcurrentDictionary<TKey, TEntity>> _initializeStoreTaskCompletionSource =  new TaskCompletionSource<ConcurrentDictionary<TKey, TEntity>>(); 
            private readonly ConcurrentDictionary<TKey, TEntity> _entities = new ConcurrentDictionary<TKey, TEntity>();

            public void Handle(AllEntitiesLoaded<TEntity, TKey> theEvent)
            {
                foreach (var entity in theEvent.Entities)
                {
                    _entities.TryAdd(entity.Id, entity);
                }

                _initializeStoreTaskCompletionSource.SetResult(_entities);
            }
        }


    }
}