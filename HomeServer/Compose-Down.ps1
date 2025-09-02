# Takes down ALL containers.
docker rm -f $(docker ps -aq)