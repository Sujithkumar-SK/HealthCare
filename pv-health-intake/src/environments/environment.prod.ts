export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com/api',
  msalConfig: {
    auth: {
      clientId: 'f834d2aa-4a16-4cf8-a3ea-205841db02f2',
      authority: 'https://login.microsoftonline.com/61330b40-bb04-4d25-b959-f3700fbe6023',
      redirectUri: 'https://your-production-url.com/auth/callback'
    }
  }
};
