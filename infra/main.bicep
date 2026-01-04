@description('Prefijo de nombres (ej: xipelabs-dev)')
param namePrefix string
param location string = resourceGroup().location

// Storage Account (para blobs)
var storageName = toLower(replace('${namePrefix}stg', '-', ''))

resource stg 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

// Container para evidencias
resource claimsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${stg.name}/default/claims'
}

// Plan consumo para Functions
resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${namePrefix}-func-plan'
  location: location
  sku: { name: 'Y1', tier: 'Dynamic' }
}

// Function App (Linux, .NET 8 isolated)
resource func 'Microsoft.Web/sites@2023-12-01' = {
  name: toLower('${namePrefix}-api')
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      appSettings: [
        // OJO: Para el primer MVP, esta cadena queda incompleta.
        // Luego la corregimos con el connection string real (o MI).
        { name: 'AzureWebJobsStorage', value: 'UseDevelopmentStorage=true' }
        { name: 'FUNCTIONS_WORKER_RUNTIME', value: 'dotnet-isolated' }
        { name: 'WEBSITE_RUN_FROM_PACKAGE', value: '1' }

        // Usaremos esto en la API para formar URI del storage
        { name: 'STORAGE_ACCOUNT', value: stg.name }
      ]
    }
  }
}

output storageAccountName string = stg.name
output functionAppName string = func.name
