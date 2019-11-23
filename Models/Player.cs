using System;

namespace UndoneAspNetCoreApi.Models
{
    public class Player : IPlayer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsBanned { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreationTime { get; set; }
        public Run[] Runs { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
        }

        public Player(NewPlayer newPlayer)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
            Name = newPlayer.Name;
            Password = newPlayer.Password;
            IsBanned = false;
            IsAdmin = false;
        }

        //get information about the player without revealing passwords
        public PlayerInformation GetPlayerInformation()
        {
            PlayerInformation playerInformation = new PlayerInformation();
            playerInformation.Id = Id;
            playerInformation.Name = Name;
            playerInformation.IsBanned = IsBanned;
            playerInformation.IsAdmin = IsAdmin;
            playerInformation.CreationTime = CreationTime;
            playerInformation.Runs = Runs;
            return playerInformation;
        }

    }
}
