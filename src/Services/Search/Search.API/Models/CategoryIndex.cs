namespace Search.API.Models
{
    public sealed class CategoryIndex
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get;  set; } = string.Empty;
        public string? Description { get;  set; }
        public string? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get;  set; }
        public int DisplayOrder { get;  set; }
    }
}
