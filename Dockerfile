FROM mcr.microsoft.com/dotnet/runtime:7.0

RUN apt-get update && apt-get install -y \
    procps  \
    && rm -rf /var/lib/apt/lists/*