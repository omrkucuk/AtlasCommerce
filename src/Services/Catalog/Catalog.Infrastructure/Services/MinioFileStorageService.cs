using Catalog.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public sealed class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioFileStorageService> _logger;
        private readonly string _baseUrl;

        public MinioFileStorageService(
            IMinioClient minioClient, 
            IConfiguration configuration,
            ILogger<MinioFileStorageService> logger)
        {
            _minioClient = minioClient;
            _logger = logger;

            _baseUrl = configuration["MinIO:BaseUrl"] ?? "http://localhost:9000";
        }

        public async Task DeleteAsync(string fileName, string bucketName, CancellationToken cancellationToken)
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation("Dosya silindi. Bucket: {Bucket}, FileName: {FileName}", bucketName, fileName);
        }

        public async Task<string> UploadAsync(
            Stream fileStream, 
            string fileName, 
            string contentType, 
            string bucketName, 
            CancellationToken cancellationToken)
        {
            // Bucket yoksa oluştur
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!exists)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);

                // Yeni bucket'ı public yap
                var policy = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [{
                        "Effect": "Allow",
                        "Principal": {"AWS": ["*"]},
                        "Action": ["s3:GetObject"],
                        "Resource": ["arn:aws:s3:::{{bucketName}}/*"]
                    }]
                }
                """;

                var setPolicyArgs = new SetPolicyArgs()
                    .WithBucket(bucketName)
                    .WithPolicy(policy);

                await _minioClient.SetPolicyAsync(setPolicyArgs, cancellationToken);
            }

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation("Dosya yüklendi. Bucket: {Bucket}, FileName: {FileName}", bucketName, fileName);

            // Public Url
            return $"{_baseUrl}/{bucketName}/{fileName}";
        }
    }
}
