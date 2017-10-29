#!/bin/bash

if [ $# -lt 1 ];
then
    echo "options: build, publish"
    exit 1
fi

if [ $1 = build ];
then
    echo "building HttpLogger..."
    dotnet build HttpLogger/HttpLogger.csproj
    
    echo "building HttpLoggerJob..."
    dotnet build HttpLoggerJob/HttpLoggerJob.csproj
    
    exit 1
fi

if [ $1 = "publish" ];
then
    echo "publishing HttpLogger to ~/dev/deploy-azure/..."
    dotnet publish HttpLogger/HttpLogger.csproj -o ~/dev/deploy-azure/
    
    echo "publishing HttpLoggerJob..."
    dotnet publish HttpLoggerJob/HttpLoggerJob.csproj -o ~/dev/deploy-azure/App_Data/jobs/triggered/web-job-01/
    
    exit 1
fi

echo "options: build, publish"
