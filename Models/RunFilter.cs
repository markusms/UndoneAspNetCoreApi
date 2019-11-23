namespace UndoneAspNetCoreApi.Models
{
    public class RunFilter
    {
        public string Level { get; set; }
        public DateFilter DateFilter { get; set; }
        public int amountOfItems { get; set; }
    }
}
