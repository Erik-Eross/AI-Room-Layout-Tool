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
set LLAMA_URL=https://github.com/ggml-org/llama.cpp/releases/download/b7845/llama-b7845-bin-win-cpu-x64.zip
set LLAMA_ZIP=llama.zip

if not exist "llama-server.exe" (
    echo Downloading llama.cpp server...
    curl -L --fail -o "%LLAMA_ZIP%" "%LLAMA_URL%"
    if errorlevel 1 (
        echo ERROR: Failed to download llama.cpp server.
        pause
        exit /b 1
    )

    echo Extracting server...
    tar -xf "%LLAMA_ZIP%"
    if errorlevel 1 (
        echo ERROR: Failed to extract llama.cpp server.
        pause
        exit /b 1
    )

    del "%LLAMA_ZIP%"
) else (
    echo llama-server already exists.
)

REM ---- Download model ----
set MODEL_URL=https://huggingface.co/about0/qwen-chat-GGUF-14B/resolve/main/qwen-chat-14B-Q4_K_M.gguf?download=true
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