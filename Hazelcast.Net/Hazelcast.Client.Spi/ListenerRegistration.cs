﻿// Copyright (c) 2008-2020, Hazelcast, Inc. All Rights Reserved.
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

using System;
using System.Collections.Concurrent;
using Hazelcast.Client.Network;
using Hazelcast.Client.Protocol;
using Hazelcast.Util;

namespace Hazelcast.Client.Spi
{
    class ListenerRegistration
    {
        public Guid UserRegistrationId { get; }
        public ClientMessage RegistrationRequest { get; }
        public DecodeRegisterResponse DecodeRegisterResponse { get; }
        public EncodeDeregisterRequest EncodeDeregisterRequest { get; }
        public DistributedEventHandler EventHandler { get; }

        public ConcurrentDictionary<Connection, EventRegistration> ConnectionRegistrations { get; } =
            new ConcurrentDictionary<Connection, EventRegistration>();

        public ListenerRegistration(Guid userRegistrationId, ClientMessage registrationRequest = null,
            DecodeRegisterResponse decodeRegisterResponse = null, EncodeDeregisterRequest encodeDeregisterRequest = null,
            DistributedEventHandler eventHandler = null)
        {
            UserRegistrationId = userRegistrationId;
            RegistrationRequest = registrationRequest;
            EncodeDeregisterRequest = encodeDeregisterRequest;
            DecodeRegisterResponse = decodeRegisterResponse;
            EventHandler = eventHandler;
        }

        protected bool Equals(ListenerRegistration other)
        {
            return string.Equals(UserRegistrationId, other.UserRegistrationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ListenerRegistration) obj);
        }

        public override int GetHashCode()
        {
            return UserRegistrationId != null ? UserRegistrationId.GetHashCode() : 0;
        }
    }
}