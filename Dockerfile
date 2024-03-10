FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

RUN apk update && apk add \
        chromium-chromedriver

ARG dotnet_cli_home_arg=/tmp/
ENV DOTNET_CLI_HOME=$dotnet_cli_home_arg