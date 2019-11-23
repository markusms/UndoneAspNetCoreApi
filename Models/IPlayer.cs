namespace UndoneAspNetCoreApi.Models
{
    public interface IPlayer
    {
        string Name { get; }
        string Password { get; }
        bool IsAdmin { get; }
    }
}
