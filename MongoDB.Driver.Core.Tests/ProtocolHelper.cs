﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Protocol;

namespace MongoDB.Driver.Core
{
    public static class ProtocolHelper
    {
        public static IRequestMessage BuildRequestMessage(string commandName)
        {
            var request = new BufferedRequestMessage();

            var queryMessage = new QueryMessage(
                new MongoNamespace("test", MongoNamespace.CommandCollectionName),
                new BsonDocument(commandName, 1),
                QueryFlags.AwaitData,
                0,
                1,
                null,
                new BsonBinaryWriterSettings());

            request.AddMessage(queryMessage);
            return request;
        }

        public static ReplyMessage BuildReplyMessage(IEnumerable<BsonDocument> documents, int responseTo = 0)
        {
            var stream = new MemoryStream();
            int docCount = 0;
            using (var writer = BsonWriter.Create(stream))
            {
                foreach (var document in documents)
                {
                    docCount++;
                    BsonSerializer.Serialize(writer, document);
                }
            }

            stream.Position = 0;
            return new ReplyMessage(
                (int)(4 + 4 + 4 + 4 + 4 + 8 + 4 + 4 + stream.Length),
                0,
                responseTo,
                OpCode.Reply,
                ReplyFlags.AwaitCapable,
                0,
                0,
                docCount,
                stream);
        }

        public static BsonDocument ReadQueryMessage(BufferedRequestMessage request)
        {
            var stream = request.Stream;
            var streamReader = new BsonStreamReader(stream);

            // assuming the query message started at buffer position 0...
            var currentPosition = stream.Position;
            stream.Position = 0;
            var length = streamReader.ReadBsonInt32();
            var requestId = streamReader.ReadBsonInt32();
            var responseTo = streamReader.ReadBsonInt32();
            var opCode = (OpCode)streamReader.ReadBsonInt32();
            var queryFlags = (QueryFlags)streamReader.ReadBsonInt32();
            var collectionFullName = streamReader.ReadBsonCString();
            var numToSkip = streamReader.ReadBsonInt32();
            var numToLimit = streamReader.ReadBsonInt32();

            BsonDocument query;
            using (var reader = BsonReader.Create(stream))
            {
                query = BsonSerializer.Deserialize<BsonDocument>(reader);
            }

            stream.Position = currentPosition;

            return query;
        }

        private class DummyRequestMessage : IRequestMessage
        {
            private readonly int _length;
            private readonly int _requestId;

            public DummyRequestMessage(int requestId, int length, string commandName)
            {
                _requestId = requestId;
                _length = length;
            }

            public int Length
            {
                get { return _length; }
            }

            public int RequestId
            {
                get { return _requestId; }
            }

            public void WriteTo(System.IO.Stream stream)
            {
                // do nothing...
            }
        }

    }
}