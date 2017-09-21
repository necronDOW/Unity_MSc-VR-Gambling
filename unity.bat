@echo off
SET mypath=%~dp0
START "" /D "C:\Program Files\Unity2017\Editor" "Unity.exe" -projectPath %mypath:~0,-1%