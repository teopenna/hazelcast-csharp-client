using Hazelcast.Client.Protocol;
using Hazelcast.Client.Protocol.Util;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using System.Collections.Generic;

namespace Hazelcast.Client.Protocol.Codec
{
    internal sealed class RingbufferSizeCodec
    {

        public static readonly RingbufferMessageType RequestType = RingbufferMessageType.RingbufferSize;
        public const int ResponseType = 103;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly RingbufferMessageType TYPE = RequestType;
            public string name;

            public static int CalculateDataSize(string name)
            {
                int dataSize = ClientMessage.HeaderSize;
                dataSize += ParameterUtil.CalculateDataSize(name);
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest(string name)
        {
            int requiredDataSize = RequestParameters.CalculateDataSize(name);
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.Set(name);
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        //************************ RESPONSE *************************//


        public class ResponseParameters
        {
            public long response;
        }

        public static ResponseParameters DecodeResponse(IClientMessage clientMessage)
        {
            ResponseParameters parameters = new ResponseParameters();
            long response ;
            response = clientMessage.GetLong();
            parameters.response = response;
            return parameters;
        }

    }
}