using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Search.API.Models;

namespace Search.API.Services
{
    public sealed class ElasticsearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly ILogger<ElasticsearchService> _logger;

        private const string ProductsIndex = "products";
        private const string CategoriesIndex = "categories";
        private const string BrandsIndex = "brands";

        public ElasticsearchService(
            ElasticsearchClient client,
            ILogger<ElasticsearchService> logger)
        {
            _client = client;
            _logger = logger;
        }


        // Uygulama başlarken index'lerin var olduğundan emin olur.Yoksa oluşturur.
        public async Task EnsureIndicesExistAsync()
        {
            await EnsureIndexAsync(ProductsIndex);
            await EnsureIndexAsync(CategoriesIndex);
            await EnsureIndexAsync(BrandsIndex);
        }

        private async Task EnsureIndexAsync(string indexName)
        {
            var exists = await _client.Indices.ExistsAsync(indexName);
            if (exists.Exists) return;

            var response = await _client.Indices.CreateAsync(indexName);

            if (response.IsSuccess())
                _logger.LogInformation("{Index} index oluşturuldu.", indexName);
            else
                _logger.LogError("{Index} index oluşturulamadı: {Error}",
                    indexName, response.ElasticsearchServerError?.Error?.Reason);
        }

        // ── Product Index İşlemleri ──

        public async Task IndexProductAsync(ProductIndex product, CancellationToken ct)
        {
            var response = await _client.IndexAsync(product, i => i
                .Index(ProductsIndex)
                .Id(product.Id), ct);

            if (!response.IsSuccess())
                _logger.LogError("Ürün index'lenemedi. Id: {Id}, Hata: {Error}",
                    product.Id, response.ElasticsearchServerError?.Error);
        }

        public async Task DeleteProductAsync(string productId, CancellationToken ct)
        {
            var response = await _client.DeleteAsync(ProductsIndex, productId, ct);

            if (response.IsSuccess())
                _logger.LogInformation("Ürün index'ten silindi. Id: {Id}", productId);
            else
                _logger.LogError("Ürün index'ten silinemedi. Id: {Id}", productId);
        }

        // -- Category Index İşlemleri --
        public async Task IndexCategoryAsync(CategoryIndex category, CancellationToken ct)
        {
            var response = await _client.IndexAsync(category, i => i
                .Index(CategoriesIndex)
                .Id(category.Id), ct);

            if (response.IsSuccess())
                _logger.LogInformation("Kategori index'lendi. Id: {Id}", category.Id);
            else
                _logger.LogError("Kategori index'lenemedi. Id: {Id}", category.Id);
        }

        public async Task DeleteCategoryAsync(string categoryId, CancellationToken ct)
        {
            var response = await _client.DeleteAsync(CategoriesIndex, categoryId, ct);

            if (response.IsSuccess())
                _logger.LogInformation("Kategori index'ten silindi. Id: {Id}", categoryId);
            else
                _logger.LogError("Kategori index'ten silinemedi. Id: {Id}", categoryId);
        }

        // -- Brand Index İşlemleri --
        public async Task IndexBrandAsync(BrandIndex brand, CancellationToken ct)
        {
            var response = await _client.IndexAsync(brand, i => i
                .Index(BrandsIndex)
                .Id(brand.Id), ct);

            if (response.IsSuccess())
                _logger.LogInformation("Marka index'lendi. Id: {Id}", brand.Id);
            else
                _logger.LogError("Marka index'lenemedi. Id: {Id}", brand.Id);
        }

        public async Task DeleteBrandAsync(string brandId, CancellationToken ct)
        {
            var response = await _client.DeleteAsync(BrandsIndex, brandId, ct);

            if (response.IsSuccess())
                _logger.LogInformation("Marka index'ten silindi. Id: {Id}", brandId);
            else
                _logger.LogError("Marka index'ten silinemedi. Id: {Id}", brandId);
        }


        // ── Arama ──

        // Product Search
        public async Task<SearchResult<ProductIndex>> SearchProductsAsync(ProductSearchRequest request, CancellationToken ct)
        {
            bool hasText = !string.IsNullOrWhiteSpace(request.Query);
            bool hasFilter = request.CategoryId.HasValue || request.BrandId.HasValue ||
                request.MinPrice.HasValue || request.MaxPrice.HasValue ||
                request.IsActive.HasValue || request.IsFeatured.HasValue ||
                request.InStock == true;

            SearchResponse<ProductIndex> response;

            if (!hasText && !hasFilter)
            {
                response = await _client.SearchAsync<ProductIndex>(s => s
                    .Indices(ProductsIndex)
                    .From((request.Page - 1) * request.PageSize)
                    .Size(request.PageSize)
                    .Query(q => q.MatchAll(m => { }))
                    .Sort(BuildSort(request)), ct);
            }
            else if (hasText && !hasFilter)
            {
                response = await _client.SearchAsync<ProductIndex>(s => s
                    .Indices(ProductsIndex)
                    .From((request.Page - 1) * request.PageSize)
                    .Size(request.PageSize)
                    .Query(q => q.MultiMatch(m => m
                        .Query(request.Query!)
                        .Fields(new[] { "name^3", "description", "sku^2", "brandName", "categoryName" })
                        .Fuzziness(new Fuzziness("AUTO"))))
                    .Sort(BuildSort(request)), ct);
            }
            else if (!hasText && hasFilter)
            {
                // Sadece filter — MatchAll + Filter
                response = await _client.SearchAsync<ProductIndex>(s => s
                    .Indices(ProductsIndex)
                    .From((request.Page - 1) * request.PageSize)
                    .Size(request.PageSize)
                    .Query(q => q.Bool(b =>
                    {
                        b.Must(m => m.MatchAll(_ => { }));
                        ApplyFilters(b, request);
                    }))
                    .Sort(BuildSort(request)), ct);
            }
            else
            {
                // Hem text hem filter
                response = await _client.SearchAsync<ProductIndex>(s => s
                    .Indices(ProductsIndex)
                    .From((request.Page - 1) * request.PageSize)
                    .Size(request.PageSize)
                    .Query(q => q.Bool(b =>
                    {
                        b.Must(m => m.MultiMatch(mm => mm
                            .Query(request.Query!)
                            .Fields(new[] { "name^3", "description", "sku^2", "brandName", "categoryName" })
                            .Fuzziness(new Fuzziness("AUTO"))));
                        ApplyFilters(b, request);
                    }))
                    .Sort(BuildSort(request)), ct);
            }

            if (!response.IsSuccess())
            {
                _logger.LogError("Arama başarısız: {Error}", response.ElasticsearchServerError?.Error);
                return new SearchResult<ProductIndex>([], 0);
            }

            return new SearchResult<ProductIndex>(
                response.Documents.ToList(),
                response.Total);
        }

        private static void ApplyFilters(BoolQueryDescriptor<ProductIndex> b,ProductSearchRequest request)
        {
            if (request.CategoryId.HasValue)
                b.Filter(f => f.Term(t => t
                    .Field(p => p.CategoryId)
                    .Value(request.CategoryId.Value.ToString())));

            if (request.BrandId.HasValue)
                b.Filter(f => f.Term(t => t
                    .Field(p => p.BrandId)
                    .Value(request.BrandId.Value.ToString())));

            if (request.IsActive.HasValue)
                b.Filter(f => f.Term(t => t
                    .Field(p => p.IsActive)
                    .Value(request.IsActive.Value)));

            if (request.IsFeatured.HasValue)
                b.Filter(f => f.Term(t => t
                    .Field(p => p.IsFeatured)
                    .Value(request.IsFeatured.Value)));

            if (request.InStock == true)
                b.Filter(f => f.Range(r => r.Number(n => n
                    .Field(p => p.StockQuantity)
                    .Gt(0))));

            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
                b.Filter(f => f.Range(r => r.Number(n =>
                {
                    n.Field(p => p.BasePrice);
                    if (request.MinPrice.HasValue) n.Gte((double)request.MinPrice.Value);
                    if (request.MaxPrice.HasValue) n.Lte((double)request.MaxPrice.Value);
                })));
        }

        // Category Search
        public async Task<SearchResult<CategoryIndex>> SearchCategoryAsync(string? query, bool? isActive, int page, int pageSize, CancellationToken ct)
        {
            SearchResponse<CategoryIndex> response;

            bool hasText = !string.IsNullOrWhiteSpace(query);
            bool hasFilter = isActive.HasValue;

            if(!hasText && !hasFilter)
            {
                response = await _client.SearchAsync<CategoryIndex>(s => s
                    .Indices(CategoriesIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.MatchAll(m => { })), ct);
            }

            else if (hasText && !hasFilter)
            {
                response = await _client.SearchAsync<CategoryIndex>(s => s
                    .Indices(CategoriesIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.MultiMatch(m => m
                        .Query(query!)
                        .Fields(new[] { "name^2", "description" })
                        .Fuzziness(new Fuzziness("AUTO")))), ct);
            }

            else if(!hasText && hasFilter)
            {
                response = await _client.SearchAsync<CategoryIndex>(s => s
                    .Indices(CategoriesIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.Bool(b => b
                        .Must(m => m.MatchAll(_ => { }))
                        .Filter(f => f.Term(t => t
                            .Field(p => p.IsActive)
                            .Value(isActive!.Value))))), ct);
            }
            else
            {
                response = await _client.SearchAsync<CategoryIndex>(s => s
                    .Indices(CategoriesIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.Bool(b => b
                        .Must(m => m.MultiMatch(mm => mm
                            .Query(query!)
                            .Fields(new[] { "name^2", "description" })
                            .Fuzziness(new Fuzziness("AUTO"))))
                        .Filter(f => f.Term(t => t
                            .Field(p => p.IsActive)
                            .Value(isActive!.Value))))), ct);
            }

            if(!response.IsSuccess())
            {
                _logger.LogError("Kategori arama başarısız: {Error}",response.ElasticsearchServerError?.Error?.Reason);
                return new SearchResult<CategoryIndex>([], 0);
            }

            return new SearchResult<CategoryIndex>(response.Documents.ToList(), response.Total);
        }

        // Brand Search
        public async Task<SearchResult<BrandIndex>> SearchBrandsAsync(string? query, bool? isActive, int page, int pageSize, CancellationToken ct)
        {
            SearchResponse<BrandIndex> response;

            bool hasText = !string.IsNullOrWhiteSpace(query);
            bool hasFilter = isActive.HasValue;

            if (!hasText && !hasFilter)
            {
                response = await _client.SearchAsync<BrandIndex>(s => s
                    .Indices(BrandsIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.MatchAll(m => { })), ct);
            }
            else if (hasText && !hasFilter)
            {
                response = await _client.SearchAsync<BrandIndex>(s => s
                    .Indices(BrandsIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.MultiMatch(m => m
                        .Query(query!)
                        .Fields(new[] { "name^2", "description" })
                        .Fuzziness(new Fuzziness("AUTO")))), ct);
            }
            else if (!hasText && hasFilter)
            {
                response = await _client.SearchAsync<BrandIndex>(s => s
                    .Indices(BrandsIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.Bool(b => b
                        .Must(m => m.MatchAll(_ => { }))
                        .Filter(f => f.Term(t => t
                            .Field(p => p.IsActive)
                            .Value(isActive!.Value))))), ct);
            }
            else
            {
                response = await _client.SearchAsync<BrandIndex>(s => s
                    .Indices(BrandsIndex)
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q.Bool(b => b
                        .Must(m => m.MultiMatch(mm => mm
                            .Query(query!)
                            .Fields(new[] { "name^2", "description" })
                            .Fuzziness(new Fuzziness("AUTO"))))
                        .Filter(f => f.Term(t => t
                            .Field(p => p.IsActive)
                            .Value(isActive!.Value))))), ct);
            }

            if (!response.IsSuccess())
            {
                _logger.LogError("Marka arama başarısız: {Error}",
                    response.ElasticsearchServerError?.Error?.Reason);
                return new SearchResult<BrandIndex>([], 0);
            }

            return new SearchResult<BrandIndex>(
                response.Documents.ToList(), response.Total);
        }

        private static Action<SortOptionsDescriptor<ProductIndex>> BuildSort(
        ProductSearchRequest request)
        {
            var order = request.SortOrder?.ToLower() == "desc"
                ? SortOrder.Desc
                : SortOrder.Asc;

            return request.SortBy?.ToLower() switch
            {
                "price" => s => s.Field(f => f.Field(p => p.BasePrice).Order(order)),
                "name" => s => s.Field(f => f.Field("name.keyword").Order(order)),
                "createdat" => s => s.Field(f => f.Field(p => p.CreatedAt).Order(order)),
                _ => s => s.Score(sc => sc.Order(SortOrder.Desc))  // Varsayılan: relevance
            };
        }

        // ── Autocomplete ──

        public async Task<IReadOnlyList<string>> AutocompleteAsync(string prefix, CancellationToken ct)
        {
            var response = await _client.SearchAsync<ProductIndex>(s => s
                .Indices(ProductsIndex)
                .Size(10)
                .Query(q => q.MatchPhrasePrefix(mpp => mpp
                    .Field(p => p.Name)
                    .Query(prefix)))
                , ct);

            if (!response.IsSuccess())
            {
                _logger.LogError("Autocomplete başarısız: {Error}",
                    response.ElasticsearchServerError?.Error?.Reason);
                return [];
            }

            return response.Documents
                .Select(d => d.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();
        }
    }
}
