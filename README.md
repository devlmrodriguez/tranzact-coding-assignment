# README

Before building our docker image, we need to publish the .NET Core 5 app with the following command inside the project folder:

(Project folder is `./TranzactCodingAssignment` in case we are in the root folder)

`cd ./TranzactCodingAssignment`

`dotnet publish c -Release`

After this, we can build the docker image as follows:

`docker build -t tranzact-image -f Dockerfile .`

Next, let's create the container from this image:

`docker create --name tranzact-container tranzact-image`

And finally, let's run the container and attach it to the current terminal:

`docker start tranzact-container -a`

**Developed by Luis Andrés Morón Rodríguez, 2020 for Tranzact Coding Assignment.**
