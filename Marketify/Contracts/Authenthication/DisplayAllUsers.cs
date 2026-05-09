namespace Marketify.Contracts.Authenthication
{
    public class DisplayAllUsers
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
