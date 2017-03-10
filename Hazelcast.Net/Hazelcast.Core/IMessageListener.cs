// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Hazelcast.Core
{
    /// <summary>
    ///     Message listener for
    ///     <see cref="ITopic{E}">ITopic&lt;E&gt;</see>
    ///     .
    /// </summary>
    public interface IMessageListener<T> : IEventListener
    {
        /// <summary>Invoked when a message is received for the added topic.</summary>
        /// <remarks>
        ///     Invoked when a message is received for the added topic. Note that topic guarantees message ordering.
        ///     Therefore there is only one thread invoking onMessage. The user shouldn't keep the thread busy and preferably
        ///     dispatch it via an Executor. This will increase the performance of the topic.
        /// </remarks>
        /// <param name="message">received message</param>
        void OnMessage(Message<T> message);
    }
}