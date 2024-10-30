```json
## Setting env

{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "YourDevelopmentConnectionStringHere"
     },
  
  "ApplicationInsights": {
      "InstrumentationKey": "YourSecretKeyHere"
    },
  "AllowedHosts": "*"
}
