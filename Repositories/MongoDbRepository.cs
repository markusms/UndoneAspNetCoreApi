using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UndoneAspNetCoreApi.Constants;
using UndoneAspNetCoreApi.Exceptions;
using UndoneAspNetCoreApi.Extensions;
using UndoneAspNetCoreApi.Models;

namespace UndoneAspNetCoreApi.Repositories
{
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoCollection<Player> collection;
        private readonly IMongoCollection<BsonDocument> bsonDocumentCollection;

        public MongoDbRepository()
        {
            //var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoClient = new MongoClient(DatabaseConsts.databaseString);
            var database = mongoClient.GetDatabase("undone");
            collection = database.GetCollection<Player>("players");
            bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
        }

        //players

        private async Task<Player> GetPlayer(Guid playerId)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var player = await collection.Find(filter).FirstAsync();
            if (player == null)
                throw new NotFoundException();
            return player;
        }

        public async Task<PlayerInformation> GetPlayerInformation(Guid playerId)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var player = await collection.Find(filter).FirstAsync();
            if (player == null)
                throw new NotFoundException();
            return player.GetPlayerInformation();
        }

        public async Task<PlayerInformationArrayHolder> GetAllPlayers()
        {
            var players = await collection.Find(new BsonDocument()).ToListAsync();
            List<PlayerInformation> playerList = new List<PlayerInformation>();
            foreach (var player in players)
            {
                playerList.Add(player.GetPlayerInformation());
            }
            var playerArray = new PlayerInformationArrayHolder();
            playerArray.InfoArray = playerList.ToArray();
            return playerArray;
        }

        public async Task<PlayerInformationArrayHolder> GetAllPlayers(string name)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var players = await collection.Find(filter).ToListAsync();
            if (players == null)
                throw new NotFoundException();

            List<PlayerInformation> playerList = new List<PlayerInformation>();
            foreach (var player in players)
            {
                playerList.Add(player.GetPlayerInformation());
            }
            var playerArray = new PlayerInformationArrayHolder();
            playerArray.InfoArray = playerList.ToArray();
            return playerArray;
        }

        public async Task<PlayerInformationArrayHolder> GetHighestScoringPlayers(string filterDate, string filterLevel, int amountOfScores)
        {
            if (amountOfScores < 1) //show a minimum of top 1
                amountOfScores = 1;
            var players = await collection.Find(FilterDefinition<Player>.Empty).ToListAsync();
            if (filterDate == "today")
            {
                foreach (var player in players)
                {
                    var list = new List<Run>(player.Runs);
                    list = list.OrderBy(x => x.TimeTaken).ToList();
                    list = list.Where(val => DateTime.Today.Subtract(val.TimePosted).TotalDays < 1).ToList();
                    player.Runs = list.ToArray();
                }
            }
            else if (filterDate == "week")
            {
                foreach (var player in players)
                {
                    if (player.Runs.Count() == 0)
                        continue;
                    var list = new List<Run>(player.Runs);
                    list = list.OrderBy(x => x.TimeTaken).ToList();
                    list = list.Where(val => DateTime.Today.Subtract(val.TimePosted).TotalDays < 7).ToList();
                    player.Runs = list.ToArray();
                }
            }

            foreach (var player in players)
            {
                var list = new List<Run>(player.Runs);
                list = list.OrderBy(x => x.TimeTaken).ToList();
                player.Runs = list.ToArray();
            }
            players = players.OrderBy(x => x.Runs[0].TimeTaken).ToList();

            List<PlayerInformation> playerList = new List<PlayerInformation>();
            foreach (var player in players)
            {
                playerList.Add(player.GetPlayerInformation());
            }
            while (playerList.Count > amountOfScores)
                playerList.RemoveAt(playerList.Count - 1);

            var playerArray = new PlayerInformationArrayHolder();
            playerArray.InfoArray = playerList.ToArray();
            return playerArray;
        }


        public async Task<PlayerInformation> CreatePlayer(Player player)
        {
            await collection.InsertOneAsync(player);
            var playerInfo = player.GetPlayerInformation();
            return playerInfo;
        }

        /// <summary>
        /// Can ban a player or give the player admin privileges.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="modifiedPlayer"></param>
        /// <returns></returns>
        public async Task<PlayerInformation> ModifyPlayer(Guid playerId, ModifiedPlayer modifiedPlayer)
        {
            Player newPlayer = await GetPlayer(playerId);
            var admin = CheckIfAdmin(modifiedPlayer);
            if (!admin)
                throw new NoPrivilegesException();

            newPlayer.IsBanned = modifiedPlayer.SetBanned;
            newPlayer.IsAdmin = modifiedPlayer.SetAdmin;
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            await collection.ReplaceOneAsync(filter, newPlayer);
            return newPlayer.GetPlayerInformation();
        }

        public async Task<PlayerInformation> DeletePlayer(Guid playerId, ModifiedPlayer playerThatDeletes)
        {
            Player playerToDelete = await GetPlayer(playerId);
            var allowedToDelete = IsAllowedToEdit(playerToDelete, playerThatDeletes);
            if (!allowedToDelete)
                throw new NoPrivilegesException();

            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            await collection.FindOneAndDeleteAsync(filter);
            return playerToDelete.GetPlayerInformation();
        }

        /// <summary>
        /// A player can be an admin if another admin has changed the player's IsAdmin boolean or by sending the request using the default admin name and password
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool CheckIfAdmin(IPlayer player)
        {
            if (player.IsAdmin)
                return true;
            if (player.Name == AdminConsts.adminName && player.Password == AdminConsts.adminPassword)
                return true;
            return false;
        }

        /// <summary>
        /// Player can edit his own account or an admin can edit another account
        /// </summary>
        /// <param name="playerToEdit"></param>
        /// <param name="playerThatEdits"></param>
        /// <returns></returns>
        private bool IsAllowedToEdit(IPlayer playerToEdit, IPlayer playerThatEdits)
        {
            if (playerToEdit.Name == playerThatEdits.Name && playerThatEdits.Password == playerThatEdits.Password)
                return true;
            return CheckIfAdmin(playerThatEdits);
        }

        //runs

        public async Task<Run> GetRun(Guid playerId, Guid runId)
        {
            Player player = await GetPlayer(playerId);
            foreach (var run in player.Runs)
            {
                if (run.Id == runId)
                {
                    return run;
                }
            }
            throw new NotFoundException();
        }

        public async Task<Run[]> GetAllRuns(Guid playerId)
        {
            Player player = await GetPlayer(playerId);
            return player.Runs;
        }

        public async Task<Run> CreateRun(Guid playerId, Run run)
        {
            Player player = await GetPlayer(playerId);
            if (player.IsBanned)
                throw new NoPrivilegesException($"Player {player.Name} is banned!");

            Run[] newRuns = new Run[player.Runs.NullableCount() + 1]; //custom extension method
            for (int i = 0; i < player.Runs.NullableCount(); i++)
            {
                newRuns[i] = player.Runs[i];
            }
            newRuns[newRuns.Count() - 1] = run;
            player.Runs = newRuns;
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, player.Id);
            await collection.ReplaceOneAsync(filter, player);
            return run;
        }

        public async Task<Run> ModifyRun(Guid playerId, Guid runId, ModifiedRun modifiedRun)
        {
            Player player = await GetPlayer(playerId);
            if (player.IsBanned)
                throw new NoPrivilegesException($"Player {player.Name} is banned!");

            foreach (var run in player.Runs)
            {
                if (run.Id == runId)
                {
                    run.TimeTaken = modifiedRun.TimeTaken;
                    FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, player.Id);
                    await collection.ReplaceOneAsync(filter, player);
                    return run;
                }
            }
            throw new NotFoundException();
        }

        public async Task<Run> DeleteRun(Guid playerId, Guid runId, ModifiedPlayer playerThatDeletes)
        {
            Player player = await GetPlayer(playerId);
            var allowedToDelete = IsAllowedToEdit(player, playerThatDeletes);
            if (!allowedToDelete)
                throw new NoPrivilegesException();

            foreach (var run in player.Runs.Reverse<Run>())
            {
                if (run.Id == runId)
                {
                    var newRuns = player.Runs.Where(val => val != run).ToArray();
                    player.Runs = newRuns; //remove run from the array
                    FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
                    await collection.ReplaceOneAsync(filter, player);
                    return run;
                }
            }

            throw new NotFoundException();
        }

        public async Task<RunArrayHolder> GetHighestScore(Guid playerId, string filterDate, string filterLevel)
        {
            var allRuns = await collection.Aggregate()
                    .Unwind(p => p.Runs)
                    .As<Run>()
                    .ToListAsync();

            var runArray = new RunArrayHolder();
            runArray.RunArray = allRuns.ToArray();
            return runArray;
        }

    }
}
