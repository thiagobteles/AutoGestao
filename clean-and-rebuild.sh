#!/bin/bash

echo "🧹 Limpando caches..."

# Deletar pastas bin e obj
find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null
find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null

echo "✅ Caches deletados"

echo "🔨 Limpando projeto..."
dotnet clean

echo "🔨 Restaurando pacotes..."
dotnet restore

echo "🔨 Compilando projeto..."
dotnet build

echo "✅ Projeto recompilado com sucesso!"
echo ""
echo "Agora execute: dotnet run"
