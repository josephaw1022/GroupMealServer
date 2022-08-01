#! /bin/sh

docker compose down 
docker image rm groupmealapi_myapp
docker compose up -d