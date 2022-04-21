using ExRam.Gremlinq.Core.GraphElements;

namespace TeamFinder.Model
{
    public class Person : VertexModel
    {
        public string Role { get; set; }

        public VertexProperty<string, NameMeta>? Name { get; set; }
    }
}
