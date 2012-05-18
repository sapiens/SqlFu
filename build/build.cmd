@echo off
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '..\src\packages\psake.4.1.0\tools\psake.ps1' build.ps1; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"
