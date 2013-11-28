namespace Hazelcast.Core
{
    /// <summary>
    ///     <p />Hazelcast provides distribution mechanism for publishing messages that are delivered to multiple subscribers
    ///     which is also known as publish/subscribe (pub/sub) messaging model.
    /// </summary>
    /// <remarks>
    ///     <p />Hazelcast provides distribution mechanism for publishing messages that are delivered to multiple subscribers
    ///     which is also known as publish/subscribe (pub/sub) messaging model. Publish and subscriptions are cluster-wide.
    ///     When a member subscribes for a topic, it is actually registering for messages published by any member in the
    ///     cluster,
    ///     including the new members joined after you added the listener.
    ///     <p />Messages are ordered, meaning, listeners(subscribers)
    ///     will process the messages in the order they are actually published. If cluster member M publishes messages
    ///     m1, m2, m3...mn to a topic T, then Hazelcast makes sure that all of the subscribers of topic T will receive
    ///     and process m1, m2, m3...mn in order.
    /// </remarks>
    public interface ITopic<E> : IDistributedObject
    {
        /// <summary>Returns the name of this ITopic instance</summary>
        /// <returns>name of this instance</returns>
        string GetName();

        /// <summary>Publishes the message to all subscribers of this topic</summary>
        /// <param name="message"></param>
        void Publish(E message);

        /// <summary>Subscribes to this topic.</summary>
        /// <remarks>
        ///     Subscribes to this topic. When someone publishes a message on this topic.
        ///     onMessage() function of the given IMessageListener is called. More than one message listener can be
        ///     added on one instance.
        /// </remarks>
        /// <param name="listener"></param>
        /// <returns>returns registration id.</returns>
        string AddMessageListener(IMessageListener<E> listener);

        /// <summary>Stops receiving messages for the given message listener.</summary>
        /// <remarks>
        ///     Stops receiving messages for the given message listener. If the given listener already removed,
        ///     this method does nothing.
        /// </remarks>
        /// <param name="registrationId">Id of listener registration.</param>
        /// <returns>true if registration is removed, false otherwise</returns>
        bool RemoveMessageListener(string registrationId);

        //    /**
        //     * Returns statistics of this topic,like total number of publishes/receives
        //     *
        //     * @return statistics
        //     */
        //    LocalTopicStats getLocalTopicStats();
    }
}