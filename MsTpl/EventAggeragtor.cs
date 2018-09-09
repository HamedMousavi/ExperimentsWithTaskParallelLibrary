using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace MsTpl
{

    public interface IEventAggeragtor
    {
        void Subscribe(object subscriber);
        void Publish<TEvent>(TEvent eventToPublish);
    }


    public interface ISubscriber<in T>
    {
        void OnEvent(T e);
    }


    public class EventAggeragtor : IEventAggeragtor
    {

        // Todo: Make this thread safe dictionary
        private readonly Dictionary<Type, List<WeakReference>> _subscribers;


        public EventAggeragtor()
        {
            _subscribers = new Dictionary<Type, List<WeakReference>>();
        }


        public void Subscribe(object subscriber)
        {
            var subscriptionRequests =
                subscriber.GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));

            var subscriberRef = new WeakReference(subscriber);
            foreach (var subscriptionRequest in subscriptionRequests)
            {
                GetSubscribers(subscriptionRequest).Add(subscriberRef);
            }
        }


        public void Publish<TEvent>(TEvent eventToPublish)
        {
            var expiredSubscriptions = new List<WeakReference>();
            // Todo: Why the original code uses ((typeof(ISubscriber<>).MakeGenericType(typeof(TEvent)));) instead of the type below
            var subscribers = GetSubscribers(typeof(ISubscriber<TEvent>));
            foreach (var weakReference in subscribers)
            {
                if (weakReference.IsAlive)
                {
                    var subscriber = (ISubscriber<TEvent>) weakReference.Target;

                    var context = SynchronizationContext.Current ?? new SynchronizationContext();
                    context.Post(c => subscriber.OnEvent(eventToPublish), null);
                }
                else expiredSubscriptions.Add(weakReference);
            }

            foreach (var expiredSubscription in expiredSubscriptions)
            {
                subscribers.Remove(expiredSubscription);
            }
        }


        private List<WeakReference> GetSubscribers(Type subscriptionType)
        {
            List<WeakReference> subscribers;
            if (!_subscribers.TryGetValue(subscriptionType, out subscribers))
            {
                subscribers = new List<WeakReference>();
                _subscribers.Add(subscriptionType, subscribers);
            }

            return subscribers;
        }
    }
}
