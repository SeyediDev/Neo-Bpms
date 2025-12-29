using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.IO.Compression;
using System.Linq;

namespace Neo.Bpms.UI.MVC.Features;

public static class PerformanceOptimization
{
    /// <summary>
    /// Adds performance optimizations (response compression) with environment from service provider
    /// Note: IWebHostEnvironment must be registered in services before calling this method
    /// </summary>
    public static IServiceCollection AddPerformanceOptimizations(this IServiceCollection services)
    {
        // Add response compression first
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        // Configure compression options based on environment using IWebHostEnvironment
        services.AddOptions<ResponseCompressionOptions>()
            .Configure<IWebHostEnvironment>((options, env) =>
            {
                // Base MIME types from defaults
                var mimeTypes = ResponseCompressionDefaults.MimeTypes.ToList();
                
                // Add additional MIME types
                mimeTypes.AddRange([
                    "text/css",
                    "application/javascript",
                    "application/json",
                    "application/xml",
                    "text/xml",
                    "text/plain",
                    "text/json",
                    "image/svg+xml",
                    "application/font-woff",
                    "application/font-woff2",
                    "font/woff",
                    "font/woff2"
                ]);
                
                // Exclude HTML from compression in development to allow browser refresh script injection
                if (env.IsDevelopment())
                {
                    mimeTypes.Remove("text/html");
                }
                else
                {
                    if (!mimeTypes.Contains("text/html"))
                    {
                        mimeTypes.Add("text/html");
                    }
                }
                
                options.MimeTypes = mimeTypes;
            });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        return services;
    }

    /// <summary>
    /// Adds performance optimizations (response compression) with explicit environment
    /// </summary>
    public static IServiceCollection AddPerformanceOptimizations(this IServiceCollection services, IHostEnvironment environment)
    {
        // Response Compression (Gzip & Brotli)
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            
            // Base MIME types from defaults
            var mimeTypes = ResponseCompressionDefaults.MimeTypes.ToList();
            
            // Add additional MIME types
            mimeTypes.AddRange([
                "text/css",
                "application/javascript",
                "application/json",
                "application/xml",
                "text/xml",
                "text/plain",
                "text/json",
                "image/svg+xml",
                "application/font-woff",
                "application/font-woff2",
                "font/woff",
                "font/woff2"
            ]);
            
            // Exclude HTML from compression in development to allow browser refresh script injection
            // In production, HTML can be compressed for better performance
            if (environment.IsDevelopment())
            {
                // Remove text/html from compression in development
                mimeTypes.Remove("text/html");
            }
            else
            {
                // Include text/html in production
                if (!mimeTypes.Contains("text/html"))
                {
                    mimeTypes.Add("text/html");
                }
            }
            
            options.MimeTypes = mimeTypes;
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        return services;
    }

    public static IApplicationBuilder UsePerformanceOptimizations(this IApplicationBuilder app, IHostEnvironment environment)
    {
        // IMPORTANT: UseResponseCompression must be called BEFORE UseStaticFiles
        app.UseResponseCompression();

        // Static Files with Caching Headers
        // Note: This should be called BEFORE any other UseStaticFiles calls
        var cacheMaxAge = environment.IsDevelopment() ? TimeSpan.FromMinutes(10) : TimeSpan.FromDays(365);
        
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var path = ctx.Context.Request.Path.Value?.ToLowerInvariant();
                
                // Skip caching for certain file types (e.g., HTML files)
                if (path?.EndsWith(".html") == true || path?.EndsWith(".htm") == true)
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
                    return;
                }
                
                // Cache static files for 1 year in production, 10 minutes in development
                ctx.Context.Response.Headers[HeaderNames.CacheControl] = 
                    $"public,max-age={cacheMaxAge.TotalSeconds},immutable";
                
                // Add Expires header
                ctx.Context.Response.Headers[HeaderNames.Expires] = 
                    DateTime.UtcNow.Add(cacheMaxAge).ToString("R");
                
                // Add ETag for conditional requests (based on last modified time)
                if (!ctx.Context.Response.Headers.ContainsKey(HeaderNames.ETag))
                {
                    var etag = $"\"{ctx.File.LastModified.ToUnixTimeSeconds()}-{ctx.File.Length}\"";
                    ctx.Context.Response.Headers[HeaderNames.ETag] = etag;
                }
                
                // Security headers for static files
                ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                
                // Add Vary header for proper caching with compression
                if (!ctx.Context.Response.Headers.ContainsKey(HeaderNames.Vary))
                {
                    ctx.Context.Response.Headers[HeaderNames.Vary] = "Accept-Encoding";
                }
            }
        });

        return app;
    }
}
