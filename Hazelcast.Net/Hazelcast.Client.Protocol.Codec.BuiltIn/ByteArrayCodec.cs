/*
 * Copyright (c) 2008-2020, Hazelcast, Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//package com.hazelcast.client.impl.protocol.codec.builtin;

//import com.hazelcast.client.impl.protocol.ClientMessage;

//import java.util.ListIterator;

namespace Hazelcast.Client.Protocol.Codec.BuiltIn
{
    internal static class ByteArrayCodec
    {
        public static void Encode(ClientMessage clientMessage, byte[] bytes)
        {
            clientMessage.Add(new ClientMessage.Frame(bytes));
        }

        public static byte[] Decode(ClientMessage.Frame frame)
        {
            return frame.Content;
        }

        public static byte[] Decode(ClientMessage.FrameIterator iterator)
        {
            return Decode(iterator.Next());
        }
    }
}