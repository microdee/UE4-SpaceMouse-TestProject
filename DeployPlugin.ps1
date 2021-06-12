
.\build.cmd --target CleanDeployment --no-logo
.\build.cmd --target MakeRelease --unreal-version 4.23.1 --no-logo
.\build.cmd --target MakeRelease --for-marketplace --unreal-version 4.24.3 --no-logo
.\build.cmd --target MakeRelease --for-marketplace --unreal-version 4.25.4 --no-logo
.\build.cmd --target MakeRelease --for-marketplace --unreal-version 4.26.2 --no-logo
.\build.cmd --target MakeRelease --for-marketplace --unreal-version 4.27.0 --no-logo

.\build.cmd --target MakeRelease --custom-engine-path "C:\EpicGames\UE_5.0EA" --unreal-version 5.0.0 --no-logo