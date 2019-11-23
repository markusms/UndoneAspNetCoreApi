using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UndoneAspNetCoreApi.Exceptions;
using UndoneAspNetCoreApi.Models;
using UndoneAspNetCoreApi.Repositories;

namespace UndoneAspNetCoreApi.Controllers
{
    [Route("api/players/{playerId}/runs")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private IRepository repository;

        public RunsController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("{runId}")]
        public async Task<Run> Get(Guid playerId, Guid runId)
        {
            return await repository.GetRun(playerId, runId);
        }

        [HttpGet]
        [Route("")]
        public async Task<Run[]> GetAll(Guid playerId)
        {
            return await repository.GetAllRuns(playerId);
        }

        [NoPrivilegesException]
        [HttpPost]
        [Route("")]
        public async Task<Run> Create(Guid playerId, NewRun newRun)
        {
            var run = new Run(newRun);
            return await repository.CreateRun(playerId, run);
        }

        [NoPrivilegesException]
        [HttpPut]
        [Route("{runId}")]
        public async Task<Run> Modify(Guid playerId, Guid runId, ModifiedRun run)
        {
            return await repository.ModifyRun(playerId, runId, run);
        }

        [NoPrivilegesException]
        [HttpDelete]
        [Route("{runId}")]
        public async Task<Run> Delete(Guid playerId, Guid runId, ModifiedPlayer playerThatDeletes)
        {
            return await repository.DeleteRun(playerId, runId, playerThatDeletes);
        }

        [HttpGet]
        [Route("top")]
        public async Task<RunArrayHolder> GetHighestScore(Guid playerId, [FromQuery(Name = "filterDate")] string filterDate, [FromQuery(Name = "filterLevel")] string filterLevel)
        {
            if (filterDate == null || filterDate != "today" || filterDate != "week")
            {
                filterDate = "allTime";
            }
            if (filterLevel == null || filterLevel != "one" || filterLevel != "two" || filterLevel != "two")
            {
                filterLevel = "game";
            }
            return await repository.GetHighestScore(playerId, filterDate, filterLevel);
        }
    }
}