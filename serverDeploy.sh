#!/bin/bash

APP_NAME="HttpServer"

PROJ_PATH="${APP_NAME}/${APP_NAME}.csproj"
BIN_DIR="${APP_NAME}/bin/publish"
OBJ_DIR="${APP_NAME}/obj"
ZIP_NAME="__${APP_NAME}__.tar.gz"

TEMP_DIR="/home/shared/temp"
TEMP_PATH="${TEMP_DIR}/${ZIP_NAME}"

TARGET_TEMP="~/bin/__${APP_NAME}__"
TARGET_DIR="~/bin/${APP_NAME}"

rm -rf ${BIN_DIR}
rm -rf ${OBJ_DIR}

while true; do
	if [ $# -eq 1 ]; then
		host=$1
	else
		echo "Host Name : ubuntu77"
		read -p "Enter a host name to deploy? " host
	fi

	case $host in
		ubuntu77 )
			RUNTIME="linux-x64"
			BASTION=""
			break;;
		* )
			if [ $# -eq 1 ]; then
				echo "Unknown host name."
				exit 1
			fi
			echo "Unknown host name. Try again.";;
	esac
done

echo "Compile...."
dotnet publish ${PROJ_PATH} \
	--output ${BIN_DIR} \
	--runtime ${RUNTIME} \
	--configuration Release \
	--self-contained \
	--property:PublishSingleFile=true

echo "Compress...."
rm -rf ${BIN_DIR}/.DS_Store
rm -rf ${BIN_DIR}/${APP_NAME}.mdb
tar -C ${BIN_DIR} -zcf ${ZIP_NAME} --no-xattrs .

if [ -n "$BASTION" ]; then
	echo "Upload...."
	ssh ${BASTION} "rm -rf ${TEMP_PATH}"
	scp -q ${ZIP_NAME} ${BASTION}:${TEMP_PATH}
	ssh ${BASTION} "chmod 666 ${TEMP_PATH}"
fi

source ./_functionToDeploy.sh

case $host in
	ubuntu77 )
		deploy_to $host true "http://${host}/Test" &
		;;
	* ) echo "Unknown host name.";;
esac
wait

if [ -n "$BASTION" ]; then
	ssh ${BASTION} "rm -rf ${TEMP_PATH}"
fi

rm -f ${ZIP_NAME}
rm -rf ${BIN_DIR}

echo "Done."
