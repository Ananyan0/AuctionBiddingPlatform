# Stage 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy WinnerNotificationService solution
COPY WinnerNotificationService/ ./WinnerNotificationService/

# Copy Auction.Contracts (dependency for event types)
COPY AuctionBiddingPlatform/Auction.Contracts/ ./AuctionBiddingPlatform/Auction.Contracts/

# Restore using the solution
RUN dotnet restore "./WinnerNotificationService/WinnerNotificationService.sln"

# Publish the API
RUN dotnet publish "./WinnerNotificationService/WinnerNotificationService.API/WinnerNotificationService.API.csproj" -c Release -o /app/publish

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WinnerNotificationService.API.dll"]
