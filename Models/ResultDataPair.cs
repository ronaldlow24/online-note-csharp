namespace OnlineNote.Models
{
    public record ResultDataPairBase
    {
        public bool Result { get; set; }
        public dynamic? CustomData { get; set; }
    }

    public record ResultDataPair<T> : ResultDataPairBase
    {
        public T? Data { get; set; }
    }
}
