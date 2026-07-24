namespace Search.API.Models
{
    public sealed class BrandIndex
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get;  set; } = string.Empty;
        public string? Description { get;  set; }
        public string? LogoUrl { get;  set; }
        public bool IsActive { get;  set; } = true;
    }
}
