# Dockerfile for AuctionBiddingPlatform (Bidding Service)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["AuctionBiddingPlatform/AuctionBiddingPlatform.csproj", "AuctionBiddingPlatform/"]
COPY ["AuctionBiddingPlatform.Application/AuctionBiddingPlatform.Application.csproj", "AuctionBiddingPlatform.Application/"]
COPY ["AuctionBiddingPlatform.Core/AuctionBiddingPlatform.Core.csproj", "AuctionBiddingPlatform.Core/"]
COPY ["AuctionBiddingPlatform.Infrastructure/AuctionBiddingPlatform.Infrastructure.csproj", "AuctionBiddingPlatform.Infrastructure/"]
COPY ["Auction.Contracts/Auction.Contracts.csproj", "Auction.Contracts/"]

# Restore dependencies
RUN dotnet restore "AuctionBiddingPlatform/AuctionBiddingPlatform.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/AuctionBiddingPlatform"
RUN dotnet build "AuctionBiddingPlatform.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuctionBiddingPlatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuctionBiddingPlatform.dll"]