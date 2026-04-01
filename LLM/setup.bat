@echo off
setlocal enabledelayedexpansion
cd /d "%~dp0"

echo ============================
echo Local AI Setup
echo ============================

REM ---- Create folders ----
if not exist "models" mkdir "models"
if not exist "bin" mkdir "bin"

REM ---- Download llama.cpp server ----
set LLAMA_URL=https://github.com/ggerganov/llama.cpp/releases/latest/download/llama-binaries-win-cpu-x64.zip
set LLAMA_ZIP=bin\llama.zip

if not exist "bin\llama-server.exe" (
    echo Downloading llama.cpp server...
    curl -L --fail -o "%LLAMA_ZIP%" "%LLAMA_URL%"

    echo Extracting server...
    tar -xf "%LLAMA_ZIP%" -C bin

    del "%LLAMA_ZIP%"
) else (
    echo llama-server already exists.
)

REM ---- Download model ----
set MODEL_URL=https://huggingface.co/bartowski/Qwen2.5-14B-Instruct-GGUF/resolve/main/Qwen2.5-14B-Instruct-Q4_K_M.gguf?download=true
set MODEL_PATH=models\model.gguf
set MIN_BYTES=8000000000

if exist "%MODEL_PATH%" (
    for %%A in ("%MODEL_PATH%") do set SIZE=%%~zA
    if !SIZE! LSS %MIN_BYTES% (
        echo Model too small, redownloading...
        del "%MODEL_PATH%"
    ) else (
        echo Model already exists.
        goto done
    )
)

echo Downloading model (this will take time)...
curl -L --fail --retry 3 --retry-delay 2 -o "%MODEL_PATH%" "%MODEL_URL%"

:done
echo Setup complete.
pause