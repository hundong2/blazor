# blazor
Blazor web application

## Getting start 

- make project 
    - `dotnet new blazorserver -o <name> --no-https`
- publish project 
    - `dotnet publish --configuration Release`
- run program
    - `dotnet bin/Release/net7.0/publish/BlazorAppLee.dll`
  
## docker 

- build blazorapp 

```
docker build -t blazorapp .
```

- docker run 

```
docker run -it -p 5001:5001 blazorapp
```

```
docker run -d -p 5001:5001 blazorapp
```

## docker compose

- install docker-compose

```sh
sudo apt install docker-compose -y 
```

- build 

```sh
docker-compose build
```

- run

```sh
docker-compose up
```

- build & run 

```sh
docker-compose up --build
```

## setting 

>## Tutorial/Reference
>>### Blazor tutorial
>>> https://dotnet.microsoft.com/ko-kr/learn/aspnet/blazor-tutorial/run
>>### using Nginx with ubuntu 
>>>https://learn.microsoft.com/ko-kr/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu
>>### blazor with NGINX Server
https://2kiju.tistory.com/59
>>### reference 
https://learn.microsoft.com/ko-kr/training/paths/aspnet-core-minimal-api/
https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0
https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0
https://www.youtube.com/watch?v=bXK-F-uL7Qo
### docker reference ( compose up )
https://docs.docker.com/reference/cli/docker/compose/up/

## Tutorial/Reference
### Blazor tutorial
https://dotnet.microsoft.com/ko-kr/learn/aspnet/blazor-tutorial/run

### using Nginx with ubuntu
https://learn.microsoft.com/ko-kr/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu

### blazor with NGINX Server
https://2kiju.tistory.com/59

### reference
https://learn.microsoft.com/ko-kr/training/paths/aspnet-core-minimal-api/ https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0 https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-8.0 https://www.youtube.com/watch?v=bXK-F-uL7Qo

## Daily note

> 2024-11-11 https://learn.microsoft.com/ko-kr/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio
>> https://learn.microsoft.com/ko-kr/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio


