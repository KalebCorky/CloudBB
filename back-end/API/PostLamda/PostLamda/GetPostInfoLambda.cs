using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Npgsql;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PostLamda;

public class GetPostInfoLambda
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        Model.PostRequest postRequest = new Model.PostRequest();
        int postID = 0;
        if (input.Body != null)
        {
            try
            {
                var bodyJSON = JsonConvert.DeserializeObject<Model.Post>(input.Body);
                if (bodyJSON != null)
                {
                    postID = bodyJSON.postID;
                }
            }
            catch (Exception e)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400, // Bad Request
                    Body = $"Error deserializing request body: {e.Message}"
                };
            }
        }

        var postRecord = GetPostInfo(postID);
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonConvert.SerializeObject(postRecord)
        };
    }
    private Model.Post GetPostInfo(int postID)
    {
        var resultPost = new Model.Post();
        // to-do: make this an AWS secret
        string connectionString = "Host=mycloudboard.chkwqwycyz4q.us-east-2.rds.amazonaws.com;Username=postgres;Password=KC23239!;Database=postgres";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Use parameterized query
            using var sqlCmd = new NpgsqlCommand("SELECT * FROM posts WHERE postID = @postID", connection);
            sqlCmd.Parameters.AddWithValue("@postID", postID);

            // Use ExecuteScalar for single result
            var reader = sqlCmd.ExecuteReader();

            if (reader.Read())
            {
                resultPost.postID = (int)reader["postid"];
                resultPost.content = (string)reader["postcontent"];
                resultPost.author = (string)reader["postauthor"];
                resultPost.created = (DateTime)reader["postdate"];
            }

            return resultPost;
        }
    }
}
