using System.Runtime.Serialization;

namespace UndoneAspNetCoreApi.Models
{
    public enum DateFilter
    {
        [EnumMember(Value = "allTime")]
        allTime,
        [EnumMember(Value = "today")]
        today,
        [EnumMember(Value = "week")]
        week
    }
}
