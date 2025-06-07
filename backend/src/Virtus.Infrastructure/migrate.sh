# Script para criar e aplicar migrações

# Navega para o diretório da API
cd ../Virtus.API

# Cria a migração inicial
dotnet ef migrations add InitialCreate -p ../Virtus.Infrastructure -s . -o Data/Migrations

# Aplica a migração
dotnet ef database update -p ../Virtus.Infrastructure -s .

echo "Migração criada e aplicada com sucesso!"