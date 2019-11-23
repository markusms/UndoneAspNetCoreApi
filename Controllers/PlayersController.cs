using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UndoneAspNetCoreApi.Exceptions;
using UndoneAspNetCoreApi.Models;
using UndoneAspNetCoreApi.Repositories;

namespace UndoneAspNetCoreApi.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IRepository repository;

        public PlayersController(IRepository repository)
        {
            this.repository = repository;
        }

        //player

        [HttpGet]
        [Route("{playerId}")]
        public async Task<PlayerInformation> Get(Guid playerId)
        {
            return await repository.GetPlayerInformation(playerId);
        }

        [HttpGet]
        [Route("")]
        public async Task<PlayerInformationArrayHolder> GetAll([FromQuery(Name = "name")] string name)
        {
            if (name == null)
                return await repository.GetAllPlayers();
            else
                return await repository.GetAllPlayers(name);
        }

        [HttpPost]
        [Route("")]
        public async Task<PlayerInformation> Create(NewPlayer newPlayer)
        {
            var player = new Player(newPlayer);
            return await repository.CreatePlayer(player);
        }

        [NoPrivilegesException]
        [HttpPut]
        [Route("{playerId}")]
        public async Task<PlayerInformation> Modify(Guid playerId, ModifiedPlayer modifiedPlayer)
        {
            return await repository.ModifyPlayer(playerId, modifiedPlayer);
        }

        [NoPrivilegesException]
        [HttpDelete]
        [Route("{playerId}")]
        public async Task<PlayerInformation> Delete(Guid playerId, ModifiedPlayer playerThatDeletes)
        {
            return await repository.DeletePlayer(playerId, playerThatDeletes);
        }

        [HttpGet]
        [Route("top")]
        public async Task<PlayerInformationArrayHolder> GetHighestScoringPlayers([FromQuery(Name = "filterDate")] string filterDate, [FromQuery(Name = "filterLevel")] string filterLevel, [FromQuery(Name = "amountOfPlayers")] int amountOfScores = 10)
        {
            if (filterDate == null || (filterDate != "today" && filterDate != "week"))
            {
                filterDate = "allTime";
            }
            if (filterLevel == null || filterLevel != "one" || filterLevel != "two" || filterLevel != "two")
            {
                filterLevel = "game";
            }
            return await repository.GetHighestScoringPlayers(filterDate, filterLevel, amountOfScores);
        }
    }
}