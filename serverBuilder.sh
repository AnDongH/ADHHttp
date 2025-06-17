#!/bin/bash

APP_NAME="HttpServer"

PROJ_PATH="${APP_NAME}/${APP_NAME}.csproj"
BIN_DIR="${APP_NAME}/bin/publish"
OBJ_DIR="${APP_NAME}/obj"
RUNTIME="linux-x64"

# dotnet publish 명령
dotnet publish ${PROJ_PATH} \
    --output ${BIN_DIR} \
    --runtime ${RUNTIME} \
    --configuration Release \
    --self-contained \
    --property:PublishSingleFile=true

ssh ubuntu77 "rm -r publish"
scp -r ${APP_NAME}/bin/publish ubuntu77:

