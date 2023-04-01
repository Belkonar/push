#!/usr/bin/env bash

# Note, structurizr here is a bash proxy to the docker shell for the host
# in the real world, point this to your actual place.
structurizr push -url {HOST_ORIGIN}/api -id 1 -key {KEY} -secret {SECRET} -w workspace.json
