@echo off
set WORKSPACE=%~dp0..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF=%WORKSPACE%\DataTables\luban.conf
set CODE_DIR=%WORKSPACE%\Assets\Scripts\Configuration\LubanGen
set DATA_DIR=%WORKSPACE%\Assets\StreamingAssets\LubanData

dotnet "%LUBAN_DLL%" -t client -c cs-bin -d bin --conf "%CONF%" -x outputCodeDir="%CODE_DIR%" -x outputDataDir="%DATA_DIR%"

if %errorlevel% neq 0 (
    echo Luban generation FAILED!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Luban generation SUCCESS! bye~
echo ========================================
