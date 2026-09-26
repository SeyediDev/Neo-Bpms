/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: { externalDir: true },
  output: 'export', // For static export (micro-frontend)
  distDir: 'dist',
  trailingSlash: true,
  
  // For embedding in MVC admin panel
  basePath: '/monitoring',
  assetPrefix: '/monitoring',
  
  // API proxy for development
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5000/api/:path*',
      },
      {
        source: '/hubs/:path*',
        destination: 'http://localhost:5000/hubs/:path*',
      },
    ];
  },
  
  // Headers for embedding
  async headers() {
    return [
      {
        source: '/:path*',
        headers: [
          {
            key: 'X-Frame-Options',
            value: 'SAMEORIGIN', // Allow embedding from same origin
          },
          {
            key: 'Content-Security-Policy',
            value: "frame-ancestors 'self' http://localhost:* https://localhost:*;",
          },
        ],
      },
    ];
  },
  
  // Webpack configuration for Module Federation (future)
  webpack: (config, { isServer }) => {
    // Future: Add Module Federation plugin here
    return config;
  },
};

module.exports = nextConfig;

