#!/usr/bin/env bash

echo $1

yaml2json -p "$1/pipeline.yaml" > "$1/pipeline.json"
dotnet run $1
