// This file configures the proxy for the Angular dev server
// It forwards API requests from the Angular app to the .NET backend
const PROXY_CONFIG = [
  {
    context: [
      "/api",  // Any request starting with /api will be proxied
    ],
    target: "https://localhost:7284",  // Forward requests to the .NET backend
    secure: false,  // Allow self-signed SSL certificates in development
    headers: {
      Connection: 'Keep-Alive'  // Keep the connection open for better performance
    }
  }
];

module.exports = PROXY_CONFIG;
