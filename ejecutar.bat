@echo off
title SIGEPOSTA - Sistema de la Posta de Mollepata
cd /d D:\Sistema_Posta_Mollepata

echo ==========================================
echo       SIGEPOSTA - INICIANDO SISTEMA
echo ==========================================

set "SIGEPOSTA_SQLSERVER=%SIGEPOSTA_SQLSERVER%"

if "%SIGEPOSTA_SQLSERVER%"=="" (
    for /f "tokens=2,*" %%A in ('reg query "HKCU\Environment" /v SIGEPOSTA_SQLSERVER 2^>nul') do set "SIGEPOSTA_SQLSERVER=%%B"
)

if "%SIGEPOSTA_SQLSERVER%"=="" (
    echo ERROR: No se encontro la conexion a SQL Server.
    pause
    exit /b 1
)

dotnet run --project SIGEPOSTA.Presentacion\SIGEPOSTA.Presentacion.csproj

if errorlevel 1 (
    echo.
    echo El sistema termino con un error.
    pause
)
