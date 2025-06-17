#!/bin/bash

RUN_OPTION=$1

# option 1 = c_build
# option 2 = d_build
# option 3 = run

if [ "${RUN_OPTION}" = "c_build" ]; then

    APP_NAME="StressClient"

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

elif [ "${RUN_OPTION}" = "d_build" ]; then
    # 컨테이너 확인 및 제거
    if [ "$(sudo docker ps -q -f name=stress-client)" ]; then
        echo "Stopping running container 'stress-client'..."
        sudo docker stop stress-client
    fi

    if [ "$(sudo docker ps -a -q -f name=stress-client)" ]; then
        echo "Removing container 'stress-client'..."
        sudo docker rm stress-client
    fi

    # 이미지 확인 및 제거
    if [ "$(sudo docker images -q stress-client-image)" ]; then
        echo "Removing image 'stress-client-image'..."
        sudo docker rmi stress-client-image
    fi

    # Docker 빌드 및 컨테이너 생성
    sudo docker build -t stress-client-image -f Dockerfile.shared_stress_client_ubuntu24 .
    sudo docker run -dit --name stress-client -v /mnt/c/Users/0423ADH/source/repos/TestHttpServer/StressClient/bin/publish:/app/publish stress-client-image

elif [ "${RUN_OPTION}" = "run" ]; then
    if [ "$(sudo docker ps -q -f name=stress-client)" ]; then
        echo "run 'stress-client' bash..."
        sudo docker exec -it stress-client /bin/bash
    elif [ "$(sudo docker ps -a -q -f name=stress-client)" ]; then
        echo "start container and run 'stress-client'..."
        sudo docker start stress-client
        sudo docker exec -it stress-client /bin/bash
    elif [ "$(sudo docker images -q stress-client-image)" ]; then
        echo "run container..."
        sudo docker run -dit --name stress-client -v /mnt/c/Users/0423ADH/source/repos/TestHttpServer/StressClient/bin/publish:/app/publish stress-client-image
        sudo docker exec -it stress-client /bin/bash
    else
        echo "please first build docker image... {sudo docker build -t stress-client-image -f Dockerfile.shared_stress_client_ubuntu24 .}"
    fi
else
    echo "invalid command. [client build: c_build] [docker build: d_build] [program run: run]"
fi