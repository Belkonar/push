#!/usr/bin/env bash

docker container rm bob

docker create \
  --name bob \
  --entrypoint ls \
  node:16

docker container start bob -a
