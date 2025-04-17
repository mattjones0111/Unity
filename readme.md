# Unity

## Dependencies

```
docker run -itd -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=changeit -p 5432:5432 postgres:15.12

docker run -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0 --blobPort 10000 --skipApiVersionCheck
```