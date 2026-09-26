/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: { externalDir: true },
  output: 'export', // For static export (micro-frontend)
  distDir: 'dist',
  trailingSlash: true,
  
  // For embedding in MVC admin panel
  basePath: '/editable-grid',
  assetPrefix: '/editable-grid',
  
  // API proxy for development
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5000/api/:path*',
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
            value: 'SAMEORIGIN',
          },
        ],
      },
    ];
  },
};

module.exports = nextConfig;

