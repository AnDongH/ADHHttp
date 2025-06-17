function deploy_to_dev() {
	REMOTE=$1
	NEED_TO_RESTART=$2
	TESTURL=$3

	echo ${REMOTE} ": Upload"
	ssh ${REMOTE} "rm -rf ${TARGET_TEMP}; mkdir -p ${TARGET_TEMP}"
	scp -q ${ZIP_NAME} ${REMOTE}:${TARGET_TEMP}

	echo ${REMOTE} ": Extract"
	ssh ${REMOTE} "tar -zxf ${TARGET_TEMP}/${ZIP_NAME} -C ${TARGET_TEMP}"
	ssh ${REMOTE} "rm -rf ${TARGET_TEMP}/${ZIP_NAME}"
	ssh ${REMOTE} "rm -rf ${TARGET_DIR}"
	ssh ${REMOTE} "mv ${TARGET_TEMP} ${TARGET_DIR}"

	if $NEED_TO_RESTART; then
		echo ${REMOTE} ": Restart"
		ssh ${REMOTE} "sudo systemctl restart ${APP_NAME}"
	fi

	if [ -n "$TESTURL" ]; then
		echo ${REMOTE} ": Test ==> ${TESTURL}"
		echo ${REMOTE} ":" `curl --silent --retry 30 --retry-delay 2 --retry-all-errors --connect-timeout 3 ${TESTURL}`
	fi
}

function deploy_to_aws() {
	REMOTE=$1
	NEED_TO_RESTART=$2
	TESTURL=$3

	echo ${REMOTE} ": Upload"
	ssh ${BASTION} "ssh ${REMOTE} \"rm -rf ${TARGET_TEMP}; mkdir -p ${TARGET_TEMP}\""
	ssh ${BASTION} "scp -q ${TEMP_PATH} ${REMOTE}:${TARGET_TEMP}"

	echo ${REMOTE} ": Extract"
	ssh ${BASTION} "ssh ${REMOTE} \"tar -zxf ${TARGET_TEMP}/${ZIP_NAME} -C ${TARGET_TEMP}\""
	ssh ${BASTION} "ssh ${REMOTE} \"rm -rf ${TARGET_TEMP}/${ZIP_NAME}\""
	ssh ${BASTION} "ssh ${REMOTE} \"rm -rf ${TARGET_DIR}\""
	ssh ${BASTION} "ssh ${REMOTE} \"mv ${TARGET_TEMP} ${TARGET_DIR}\""

	if $NEED_TO_RESTART; then
		echo ${REMOTE} ": Restart"
		ssh ${BASTION} "ssh ${REMOTE} \"sudo systemctl restart ${APP_NAME}\""
	fi

	if [ -n "$TESTURL" ]; then
		echo ${REMOTE} ": Test ==> ${TESTURL}"
		echo ${REMOTE} ":" `ssh ${BASTION} curl --silent --retry 30 --retry-delay 2 --retry-all-errors --connect-timeout 3 ${TESTURL}`
	fi
}

function deploy_to() {
	if [ -n "${BASTION}" ]; then
		deploy_to_aws $@
	else
		deploy_to_dev $@
	fi
}
