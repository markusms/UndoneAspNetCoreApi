using System;

namespace UndoneAspNetCoreApi.Models
{
    public interface IRun
    {
        float TimeTaken { get; }
        DateTime TimePosted { get; }
        string Level { get; }
    }
}
