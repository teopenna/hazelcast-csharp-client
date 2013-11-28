using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hazelcast.Client.Request.Collection;
using Hazelcast.Client.Spi;
using Hazelcast.Core;
using Hazelcast.IO.Serialization;
using Hazelcast.Util;

namespace Hazelcast.Client.Proxy
{
    //.Net reviewed
    public class AbstractClientCollectionProxy<E> : ClientProxy, IHazelcastCollection<E>
    {

        protected internal readonly string partitionKey;

        public AbstractClientCollectionProxy(string serviceName, string objectName) : base(serviceName, objectName)
        {
            partitionKey = GetPartitionKey();
        }

        protected internal override void OnDestroy()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<E> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool Add(E item)
        {
            ThrowExceptionIfNull(item);
            var request = new CollectionAddRequest(GetName(), ToData(item));
            return Invoke<bool>(request);
        }

        void ICollection<E>.Add(E item)
        {
            Add(item);
        }

        public void Clear()
        {
            var request = new CollectionClearRequest(GetName());
            Invoke<bool>(request);
        }

        public bool Contains(E item)
        {
            ThrowExceptionIfNull(item);
            var request = new CollectionContainsRequest(GetName(), ToData(item));
            var result = Invoke<bool>(request);
            return result;
        }

        public void CopyTo(E[] array, int arrayIndex)
        {
            GetAll().ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(E item)
        {
            ThrowExceptionIfNull(item);
            var request = new CollectionRemoveRequest(GetName(), ToData(item));
            var result = Invoke<bool>(request);
            return result;
        }

        public int Count
        {
            get { return Size(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Size()
        {
            var request = new CollectionSizeRequest(GetName());
            var result = Invoke<int>(request);
            return result;
        }

        public bool IsEmpty()
        {
            return Size() == 0;
        }

        public E[] ToArray()
        {
            E[] array = GetAll().ToArray();
            var tmp = new E[array.Length];
            Array.Copy(array, 0, tmp, 0, array.Length);
            return tmp;
        }

        public T[] ToArray<T>(T[] a)
        {
            E[] array = ToArray();
            if (a.Length < array.Length)
            {
                // Make a new array of a's runtime type, but my contents:
                var tmp= new T[array.Length];
                Array.Copy(array, 0, tmp, 0, array.Length);
                return tmp;
            }
            Array.Copy(array, 0, a, 0, array.Length);
            if (a.Length > array.Length)
                a[array.Length] = default(T);
            return a;
        }

        public bool ContainsAll<T>(ICollection<T> c)
        {
            ThrowExceptionIfNull(c);
            ICollection<Data> valueSet = new HashSet<Data>();
            foreach (object o in c)
            {
                ThrowExceptionIfNull(o);
                valueSet.Add(ToData(o));
            }
            var request = new CollectionContainsRequest(GetName(), valueSet);
            var result = Invoke<bool>(request);
            return result;
        }

        public bool RemoveAll<T>(ICollection<T> c)
        {
            return CompareAndRemove(false, c);
        }

        public bool RetainAll<T>(ICollection<T> c)
        {
            return CompareAndRemove(true, c);
        }

        public bool AddAll<T>(ICollection<T> c)
        {
            ThrowExceptionIfNull(c);
            IList<Data> valueList = new List<Data>();
            foreach (T e in c)
            {
                ThrowExceptionIfNull(e);
                valueList.Add(ToData(e));
            }
            var request = new CollectionAddAllRequest(GetName(), valueList);
            var result = Invoke<bool>(request);
            return result;
        }

        public virtual string AddItemListener(IItemListener<E> listener, bool includeValue)
        {
            var request = new CollectionAddListenerRequest(GetName(), includeValue);
            request.SetServiceName(GetServiceName());
            return Listen<PortableItemEvent>(request, GetPartitionKey(), (sender, args) => HandleItemListener(args, listener, includeValue));
        }

        private void HandleItemListener(PortableItemEvent portableItemEvent, IItemListener<E> listener, bool includeValue)
        {
            E item = includeValue ? (E)GetContext().GetSerializationService().ToObject(portableItemEvent.GetItem()) : default(E);
            IMember member = GetContext().GetClusterService().GetMember(portableItemEvent.GetUuid());
            ItemEvent<E> itemEvent = new ItemEvent<E>(GetName(), portableItemEvent.GetEventType(), item, member);
            if (portableItemEvent.GetEventType() == ItemEventType.Added){
                listener.ItemAdded(itemEvent);
            } else {
                listener.ItemRemoved(itemEvent);
            }
        }

        public bool RemoveItemListener(string registrationId)
        {
            return StopListening(registrationId);
        }

        private bool CompareAndRemove<T>(bool retain, ICollection<T> c)
        {
            ThrowExceptionIfNull(c);
            ICollection<Data> valueSet = new HashSet<Data>();
            foreach (object o in c)
            {
                ThrowExceptionIfNull(o);
                valueSet.Add(ToData(o));
            }
            var request = new CollectionCompareAndRemoveRequest(GetName(), valueSet, retain);
            var result = Invoke<bool>(request);
            return result;
        }

        protected internal virtual T Invoke<T>(object req)
        {
            var collectionRequest = req as CollectionRequest;
            if (collectionRequest != null)
            {
                CollectionRequest request = collectionRequest;
                request.SetServiceName(GetServiceName());
            }
            try
            {
                return GetContext().GetInvocationService().InvokeOnKeyOwner<T>(req, GetPartitionKey());
            }
            catch (Exception e)
            {
                throw ExceptionUtil.Rethrow(e);
            }
        }

        protected internal virtual Data ToData(object o)
        {
            return GetContext().GetSerializationService().ToData(o);
        }

        protected internal virtual object ToObject(Data data)
        {
            return GetContext().GetSerializationService().ToObject(data);
        }

        protected internal virtual void ThrowExceptionIfNull(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
        }
        protected IEnumerable<E> GetAll()
        {
            var request = new CollectionGetAllRequest(GetName());
            var result = Invoke<SerializableCollection>(request);
            ICollection<Data> collection = result.GetCollection();
            var list = new List<E>(collection.Count);
            foreach (Data value in collection)
            {
                list.Add((E)ToObject(value));
            }
            return list;
        }

    }

}