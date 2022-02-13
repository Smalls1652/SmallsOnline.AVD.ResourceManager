using Azure.Core.Serialization;
using Microsoft.Azure.Cosmos;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

/// <summary>
/// A custom serializer to serialize and deserialize data for the CosmosDB service.
/// </summary>
public class CosmosDbSerializer : CosmosSerializer
{
    private readonly JsonObjectSerializer jsonSerializer;

    public CosmosDbSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        jsonSerializer = new(jsonSerializerOptions);
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream.CanSeek
                   && stream.Length == 0)
            {
                return default!;
            }

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            return (T)jsonSerializer.Deserialize(
                stream: stream,
                returnType: typeof(T),
                cancellationToken: default
            )!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream streamPayload = new();

        jsonSerializer.Serialize(
            stream: streamPayload,
            value: input,
            inputType: typeof(T), 
            cancellationToken: default
        );
        streamPayload.Position = 0;

        return streamPayload;
    }
}