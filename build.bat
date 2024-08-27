@echo off

set CGO_ENABLED=1

echo Building go backend...
cd .\src\backend
go build -o .\bin\build\backend.exe
IF NOT EXIST .\bin\build\.env (
    echo. > .\bin\build\.env
)
echo Done!


echo Building frontend...
cd ..\frontend
pnpm build

cd ..\

if NOT EXIST .\backend\bin\build\static (
    mkdir .\backend\bin\build\static
)
robocopy .\frontend\build .\backend\bin\build\static /COPY:DAT /E