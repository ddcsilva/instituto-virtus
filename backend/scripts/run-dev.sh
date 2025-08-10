#!/bin/bash

echo "🚀 Iniciando Instituto Virtus Backend..."

# Navegar para o diretório da API
cd src/InstitutoVirtus.API

# Restaurar pacotes
echo "📦 Restaurando pacotes..."
dotnet restore

# Aplicar migrations
echo "🗄️ Aplicando migrations..."
dotnet ef database update

# Executar aplicação
echo "✅ Iniciando API..."
dotnet run

# API disponível em:
# - http://localhost:5000
# - https://localhost:5001
# - Swagger: http://localhost:5000/swagger