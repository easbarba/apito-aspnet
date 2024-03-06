# apito-aspnet is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# apito-aspnet is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with apito-aspnet. If not, see <https://www.gnu.org/licenses/>.

# DEPENDENCIES: podman, gawk, fzf, guix.

# LOAD ENV FILES
-include envs/.env.*

RUNNER ?= podman
POD_NAME := apito
NAME := apito-aspnet
VERSION := $(shell awk '/<Version>/ {version=substr($$0,14,5); print version}' ./Apito/Apito.csproj)
BACKEND_IMAGE=${GITLAB_REGISTRY}/${USER}/${NAME}:${VERSION}
BACKEND_FOLDER=/app

# ======================================= MAIN

.PHONY: up
up: initial database

.PHONY: down
down:
	${RUNNER} pod rm --force ${POD_NAME}
	${RUNNER} container rm --force ${DATABASE_NAME}
	${RUNNER} volume rm --force ${DATABASE_DATA}
	${RUNNER} container rm --force ${NAME}


# ======================================== POD

.PHONY: logs
logs:
	${RUNNER} logs ${NAME}

.PHONY: stats
stats:
	${RUNNER} pod stats ${POD_NAME}

.PHONY: initial
initial:
	${RUNNER} pod create \
		--publish ${BACKEND_PORT}:${BACKEND_INTERNAL_PORT} \
		--publish ${FRONTEND_PORT}:${FRONTEND_INTERNAL_PORT} \
		--name ${POD_NAME}

.PHONY: database
database:
	# ----------- ADD DATABASE
	${RUNNER} rm -f ${DATABASE_NAME}
	${RUNNER} run ${RUNNER_STATS} \
		--detach \
		--pod ${POD_NAME} \
		--name ${DATABASE_NAME} \
		--env POSTGRES_PASSWORD=${SQL_PASSWORD} \
		--env POSTGRES_USER=${SQL_USERNAME} \
		--env POSTGRES_DB=${SQL_DATABASE} \
		--volume ${DATABASE_DATA}:${SQL_DATA}:Z \
		${DATABASE_IMAGE}


.PHONY: database.repl
database.repl:
	# ----------- DATABASE REPL
	${RUNNER} exec -it ${DATABASE_NAME} \
		psql --username ${SQL_USERNAME} --dbname ${SQL_DATABASE}

# .PHONY: database.test
# database.test:
# 	# ----------- ADD DATABASE TESTING
# 	${RUNNER} rm -f ${DATABASE_NAME}-test
# 	${RUNNER} volume rm -f ${DATABASE_DATA}-test

# 	${RUNNER} run ${RUNNER_STATS} \
# 		--detach \
# 		--pod ${POD_NAME} \
# 		--name ${DATABASE_NAME}-test \
# 		--env POSTGRES_PASSWORD=${SQL_PASSWORD} \
# 		--env POSTGRES_USER=${SQL_USERNAME} \
# 		--env POSTGRES_DB=${SQL_DATABASE}_test \
# 		--volume ${DATABASE_DATA}-test:${SQL_DATA}:Z \
# 		${DATABASE_IMAGE}

# database.test.old:
# 	${RUNNER} exec -it ${DATABASE_NAME} \
# 		psql --username ${SQL_USERNAME} --dbname ${SQL_DATABASE} \
# 			--command 'drop database if exists ${SQL_DATABASE}_test'
# 	${RUNNER} exec -it ${DATABASE_NAME} \
# 		psql --username ${SQL_USERNAME} --dbname ${SQL_DATABASE} \
# 			--command 'create database ${SQL_DATABASE}_test'

.PHONY: system
prod:
	${RUNNER} run ${RUNNER_STATS} \
		--pod ${POD_NAME} \
		--detach \
		--name ${NAME} \
		${NAME}:${VERSION}

.PHONY: start
start:
	${RUNNER} container rm --force ${NAME}-start
	${RUNNER} run \
		--pod ${POD_NAME} \
		--rm --tty --interactive \
		--name ${NAME}-start \
		--volume ${PWD}:/app:Z \
		--workdir /app \
		--env-file ./envs/.env.db \
		${BACKEND_IMAGE} \
		bash -c './scripts/watch'

repl:
	${RUNNER} container rm --force ${NAME}-repl
	${RUNNER} run ${RUNNER_STATS} \
		--pod ${POD_NAME} \
		--rm --interactive --tty \
		--name ${NAME}-repl \
		--volume ${PWD}:/app:Z \
		--workdir /app/Apito \
		--env-file ./envs/.env.db \
		--quiet \
		${BACKEND_IMAGE} \
		bash

.PHONY: command
command:
	${RUNNER} container rm --force ${NAME}-command
	${RUNNER} run \
		--pod ${POD_NAME} \
		--rm --tty --interactive \
		--name ${NAME}-command \
		--volume ${PWD}:/app:Z \
		--workdir /app/Apito \
		--env-file ./envs/.env.db \
		--quiet \
		${BACKEND_IMAGE} \
		bash -c "dotnet $(shell cat container-commands | fzf) --project ./Apito"

.PHONY: test.integration
test.integration:
	${RUNNER} container rm --force ${NAME}-test
	${RUNNER} run \
		--pod ${POD_NAME} \
		--rm --tty --interactive \
		--name ${NAME}-test \
		--volume ${PWD}:/app:Z \
		--workdir /app \
		--env-file ./envs/.env.db \
		--env SQL_DATABASE=apito_development_test \
		${BACKEND_IMAGE} \
		bash -c './scripts/test'

.PHONY: api
api:
	./apitest | jq

# ============================================= TASKS

.PHONY: image.build
image.build:
	# ---------------------- BUILD BACKEND IMAGE
	${RUNNER} build \
		--file ./Dockerfile \
		--tag ${BACKEND_IMAGE}

.PHONY: image.publish
image.publish:
	# ---------------------- PUBLISH BACKEND IMAGE
	${RUNNER} push ${BACKEND_IMAGE}

.PHONY: system
system:
	guix shell --pure --container

.DEFAULT_GOAL := test
