using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Npgsql;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TopicLambda;

public class GetTopicInfoLambda
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public APIGatewayProxyResponse FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        Model.TopicRequest topicReq = new Model.TopicRequest();

        // Initialize topicID as an integer
        int topicID = 2;

        if (request.Body != null)
        {
            var body = request.Body;

            try
            {
                var bodyJSON = JsonConvert.DeserializeObject<Model.TopicRequest>(body);

                // Check if deserialization was successful
                if (bodyJSON != null)
                {
                    topicID = Convert.ToInt32(bodyJSON.TopicId);
                }
            }
            catch (Exception ex)
            {
                // Handle deserialization error
                // Log the exception or return an error response
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400, // Bad Request
                    Body = $"Error deserializing request body: {ex.Message}"
                };
            }
        }

        var topicRecord = GetTopicInfo(topicID); // Convert to string

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Origin", "*" }, // Allow requests from any origin
                { "Access-Control-Allow-Headers", "Content-Type" }, // Add any additional headers needed
                { "Access-Control-Allow-Methods", "GET" } // Specify the allowed HTTP methods
            },
            Body = JsonConvert.SerializeObject(topicRecord)
        };
    }

    public Model.Topic GetTopicInfo(int topicID)
    {
        var resultTopic = new Model.Topic();
        // to-do: make this an AWS secret
        string connectionString = "Host=mycloudboard.chkwqwycyz4q.us-east-2.rds.amazonaws.com;Username=postgres;Password=KC23239!;Database=postgres";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Use parameterized query
            using var sqlCmd = new NpgsqlCommand("SELECT * FROM topics WHERE topicID = @topicID", connection);
            sqlCmd.Parameters.AddWithValue("@topicID", topicID);

            // Use ExecuteScalar for single result
            var reader = sqlCmd.ExecuteReader();

            if (reader.Read())
            {
                resultTopic.topicID = (int)reader["topicid"];
                resultTopic.topicName = (string)reader["topicname"];
                resultTopic.topicAuthor = (string)reader["topiccreator"];
                // Assuming "posts" is an array type in the database, you might need to adjust this part
                resultTopic.posts = (int[])reader["posts"];
            }

            return resultTopic;
        }
    }
}