#!/bin/bash

for project_dir in src/*/
do
    project_dir=${project_dir%*/}
    echo "Publishing NuGet package: ${project_dir##*/}"
    chmod +x ./$project_dir/scripts/dotnet-pack.sh
    ./$project_dir/scripts/dotnet-pack.sh &
    wait
done

echo "Finished publishing NuGet packages."
