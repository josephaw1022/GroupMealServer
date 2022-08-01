

## Read Me




Dockerfile 
```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5000


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["GroupMealApi.csproj", "./"]
RUN dotnet restore "GroupMealApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GroupMealApi.csproj" -c Release -o /app/build

# Publish the app to the /app folder
FROM build AS publish
RUN dotnet publish "GroupMealApi.csproj" -c Release -o /app/publish /p:UseAppHost=true


# Run the app from the /app folder
FROM base AS watch 
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupMealApi.dll"]
```


Docker Compose 
```Dockerfile
services:
  # ASP .NET Core App
  myapp:
    build:
      context: .
      dockerfile: ./Dockerfile
    ports:
      - 5000:5000
    depends_on:
      - mongodb-express
    restart: always
    env_file:
      - docker.env

    volumes:
      - data:/data/app
    


  # Mongo DB
  mongodb:
    image: mongo:latest
    env_file:
      - docker.env
    ports:
      - 27017:27017
    volumes:
      - data:/data/db
    restart: always

  # Mongo Express
  mongodb-express:
    image: mongo-express:latest
    ports:
      - 8081:8081
    depends_on:
      - mongodb
    restart: always
    env_file:
      - docker.env
    volumes:
      - data:/data/db
  
    


volumes:
  data:
  
```


simpy run 
```bash 
./clear-docker.sh
```