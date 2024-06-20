namespace DataShareCore.Common.Enum;

public class ResponseCode
{
    public enum ResponseStatusCode
    {
        Ok = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 500,
        Unauthorized = 401,
        Forbidden = 403,
        InternalServerError = 500
    }
}