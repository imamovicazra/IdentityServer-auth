using System.Text.Json.Serialization;


namespace Identity.Model.DTOs.Requests
{
    public class UserUpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
