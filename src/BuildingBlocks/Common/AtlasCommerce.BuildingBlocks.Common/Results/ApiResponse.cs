using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.Common.Results
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; }

        // Validation gibi durumlarda tek bir mesaj yetmeyebilir (birden fazla alan hatası olabilir). Bu yüzden ayrıca bir Error listesi tutuyoruz.
        public IReadOnlyList<string>? Errors { get; init; }

        public static ApiResponse<T> Ok(T data, string? message = null) => 
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    // Veri taşımayan (örn. DeleteProduct sonrası 204 değil de "success: true" dönmek istediğinde) cevaplar için.
    public sealed class ApiResponse
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public IReadOnlyList<string>? Errors { get; init; }

        public static ApiResponse Ok(string? message = null) =>
            new() { Success = true, Message = message };

        public static ApiResponse Fail(string message, IReadOnlyList<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
