namespace TeamFinder.Model
{
    public class VertexModel
    {
        public object? Id { get; set; }
        public string? Label { get; set; }
        public string PartitionKey { get; set; } = "Name";
    }
}
