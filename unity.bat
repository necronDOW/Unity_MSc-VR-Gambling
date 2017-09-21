@echo off
SET mypath=%~dp0
SET /p versionraw=<%mypath%ProjectSettings\ProjectVersion.txt

FOR /f "tokens=1,2 delims=. " %%a IN ("%versionraw%") DO SET version=%%b
IF EXIST "C:\Program Files\Unity2017" IF %version% GTR 5 (
	SET unitypath="C:\Program Files\Unity2017\Editor"
) ELSE (
	SET unitypath="C:\Program Files\Unity\Editor"
)

START "" /D %unitypath% "Unity.exe" -projectPath %mypath:~0,-1%